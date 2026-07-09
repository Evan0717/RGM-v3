using Exiled.API.Features.DamageHandlers;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Features.Items;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using PlayerRoles;
using RGM.API.Features;
using RGM.API.DataBases;

using static RGM.Variables.Variable;
using Respawning;
using Exiled.API.Features.Waves;
using Door = Exiled.API.Features.Doors.Door;
using ChaosMiniWave = Respawning.Waves.ChaosMiniWave;
using NtfMiniWave = Respawning.Waves.NtfMiniWave;
using Player = Exiled.API.Features.Player;
using Round = Exiled.API.Features.Round;
using Server = Exiled.API.Features.Server;
using SpawnableWaveBase = Respawning.Waves.SpawnableWaveBase;
using Warhead = Exiled.API.Features.Warhead;

namespace RGM.Modes
{
    [Mode(ModeCategory.Private, ModeInfo.Set, ModeType.Juggernaut)]
    class Juggernaut : Mode
    {
        public override string Name => "저거너트";
        public override string Description => "모두 힘을 합쳐 외부의 적에 대항하세요.";
        public override string Detail =>
"""
지금은 우리(SCP, 반란, MTF)끼리 싸울 때가 아닙니다.
군대를 홀로 섬멸할 수 있는 전력인 저거너트가 재단을 점령하기 위해 왔습니다.

이제 친구가 될 시간이야.

* 게임 시작 12분 뒤 <color=red>자동핵</color>이 작동됩니다.
* 저거너트는 알파 핵탄두가 폭파할 경우 즉시 과부하 프로토콜이 작동합니다.
""";
        public override string Color => "088A08";

        public static Juggernaut Instance;

        Player juggernaut;
        List<Player> ScpAttackCooldown = new List<Player>();
        Dictionary<Player, float> PlayerDamages = new Dictionary<Player, float>();
        HashSet<SpawnableWaveBase> juggernautExternalSupportWaves = new HashSet<SpawnableWaveBase>();
        Speaker speaker;
        float stack = 0;

        CoroutineHandle _onModeStarted;
        CoroutineHandle _autoWarhead;
        CoroutineHandle _findLocate;
        CoroutineHandle _musicAsync;
        CoroutineHandle _reduceWaveTimer;

        public override void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
            Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;

            Exiled.Events.Handlers.Server.RespawnedTeam += OnRespawnedTeam;

            Exiled.Events.Handlers.Item.ChargingJailbird += OnChargingJailbird;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _autoWarhead = Timing.RunCoroutine(AutoWarhead());
            _findLocate = Timing.RunCoroutine(FindLocate());
            _musicAsync = Timing.RunCoroutine(MusicAsync());
            _reduceWaveTimer = Timing.RunCoroutine(ReduceWaveTimer());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.SearchingPickup -= OnSearchingPickup;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.ReceivingEffect -= OnReceivingEffect;
            Exiled.Events.Handlers.Player.Handcuffing -= OnHandcuffing;

            Exiled.Events.Handlers.Server.RespawnedTeam -= OnRespawnedTeam;

            Exiled.Events.Handlers.Item.ChargingJailbird -= OnChargingJailbird;

            juggernautExternalSupportWaves.Clear();

            Timing.KillCoroutines(_onModeStarted);
            Timing.KillCoroutines(_autoWarhead);
            Timing.KillCoroutines(_findLocate);
            Timing.KillCoroutines(_musicAsync);
            Timing.KillCoroutines(_reduceWaveTimer);

            if (speaker != null)
                speaker.Destroy();
        }

        public IEnumerator<float> JuggernautExternalTimer()
        {
            for (int i = 1; i < 75; i++)
            {
                if (Round.IsEnded)
                    yield break;

                PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(1, $"<size=25>저거너트 외부 지원 호출까지 {75 - i}초</size>"));

                yield return Timing.WaitForSeconds(1f);
            }
        }
        public IEnumerator<float> RoundTimer()
        {
            for (int i = 1; i < 821; i++)
            {
                if (Round.IsEnded || Warhead.IsDetonated)
                    yield break;

                PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(1, $"<size=25>저거너트 과부하 프로토콜 가동까지 {821 - i}초</size>"));

                yield return Timing.WaitForSeconds(1f);
            }
        }
        public IEnumerator<float> AnnihilationTimer()
        {
            for (int i = 1; i < 120; i++)
            {
                if (Round.IsEnded)
                    yield break;

                PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(1, $"<size=25>초토화 작전 실행까지 {120 - i}초</size>"));

                yield return Timing.WaitForSeconds(1f);
            }
        }

        bool IsJuggernautTeam(Player player)
        {
            return player == juggernaut || player.Role.Type == RoleTypeId.Tutorial;
        }

        bool TryGetExternalSupportWave<T>(out SpawnableWaveBase wave) where T : SpawnableWaveBase
        {
            if (WaveManager.TryGet<T>(out T spawnWave))
            {
                wave = spawnWave;
                return true;
            }

            wave = null;
            return false;
        }

        bool TryGetRandomSupportWave(out SpawnableWaveBase wave)
        {
            if (Random.Range(1, 3) == 1)
            {
                return TryGetExternalSupportWave<NtfMiniWave>(out wave) || TryGetExternalSupportWave<ChaosMiniWave>(out wave);
            }

            return TryGetExternalSupportWave<ChaosMiniWave>(out wave) || TryGetExternalSupportWave<NtfMiniWave>(out wave);
        }

        void CallJuggernautExternalSupport()
        {
            if (Round.IsEnded || !PlayerManager.List.Any(x => x.IsDead && x.Role.Type != RoleTypeId.Overwatch))
                return;

            if (!TryGetRandomSupportWave(out SpawnableWaveBase wave))
                return;

            juggernautExternalSupportWaves.Add(wave);
            Timing.CallDelayed(10f, () => juggernautExternalSupportWaves.Remove(wave));
            WaveManager.Spawn(wave);
        }

        void CallRegularSupport()
        {
            if (Round.IsEnded || !PlayerManager.List.Any(x => x.IsDead && x.Role.Type != RoleTypeId.Overwatch))
                return;

            if (!TryGetRandomSupportWave(out SpawnableWaveBase wave))
                return;

            Respawn.GrantTokens(wave.TargetFaction, 1);

            if (!WaveTimer.TryGetWaveTimers(wave.TargetFaction, out List<WaveTimer> waves))
                return;

            foreach (var w in waves)
                w.SetTime(0);
        }

        void TriggerAnnihilation()
        {
            foreach (var player in PlayerManager.List.Where(x => true))
            {
                player.Kill("패기에 의해 공중분해 되었습니다");
            }
        }
        public IEnumerator<float> OnModeStarted()
        {  
            Door.Get(DoorType.EscapeFinal).IsOpen = true;

            foreach (var player in PlayerManager.List)
            {
                Spawned(player);
            }

            juggernaut = Tools.GetRandomValue(PlayerManager.List.ToList());
            
            juggernaut.Role.Set(RoleTypeId.Tutorial);
            juggernaut.Scale = new Vector3(1.12f, 1.12f, 1.12f);
            juggernaut.MaxHealth = 580 * PlayerManager.List.Count();
            juggernaut.Health = juggernaut.MaxHealth;
            juggernaut.IsBypassModeEnabled = true;
            juggernaut.EnableEffect(EffectType.SinkHole);
            juggernaut.EnableEffect(EffectType.Slowness, 3);
            juggernaut.EnableEffect(EffectType.BodyshotReduction, 4);
            juggernaut.AddBroadcast(10, "<b><size=30>당신은 <color=#298A08>저거너트</color>입니다.</size></b>\n<size=25>본인을 제외한 모두를 사살하십시오.</size>");
            juggernaut.Position = new Vector3(123.3271f, 288.7908f, 27.01838f);

            List<ItemType> items = new List<ItemType>() {
                ItemType.GunLogicer,
                ItemType.Jailbird,
                ItemType.ArmorHeavy
            };
            foreach (var item in items)
                juggernaut.AddItem(item);
            
            // 이 아래부터 초토화 작전 부 까지, Warhead.IsDetonated 이후 작동되도록 설계해야 함.
            Timing.RunCoroutine(RoundTimer());
            while (!Round.IsEnded && !Warhead.IsDetonated)
                yield return Timing.WaitForSeconds(1f);

            if (Round.IsEnded)
                yield break;

            juggernaut.DisableAllEffects();
            juggernaut.EnableEffect(EffectType.BodyshotReduction, 4);
            juggernaut.EnableEffect(EffectType.Lightweight, 10);
            juggernaut.EnableEffect(EffectType.NightVision, 100);
            juggernaut.EnableEffect(EffectType.Scp1344, 1);
            juggernaut.EnableEffect(EffectType.Scp1853, 1);
            juggernaut.EnableEffect(EffectType.Scp207, 2);
            juggernaut.EnableEffect(EffectType.FogControl, 1);
            juggernaut.EnableEffect(EffectType.Bleeding, 5);
            juggernaut.EnableEffect(EffectType.Burned, 1);
            PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(10, "<b><size=30><color=#298A08>저거너트</color>가 과부하 상태에 돌입합니다!</size></b>\n<size=25>모든 능력치가 강화되는 대신 지속 피해를 받으며, 받는 피해가 25% 증가합니다.</size>"));
            yield return Timing.WaitForSeconds(10f);
            
            // 저거너트 외부 지원 호출 구현(2번에 나눠서)
            Timing.RunCoroutine(JuggernautExternalTimer());
            yield return Timing.WaitForSeconds(75f); // 1차 호출
            CallJuggernautExternalSupport();

            Timing.RunCoroutine(JuggernautExternalTimer());
            yield return Timing.WaitForSeconds(75f); // 2차 호출
            CallJuggernautExternalSupport();
            
            // 초토화 작전 구현
            Timing.RunCoroutine(AnnihilationTimer()); 
            yield return Timing.WaitForSeconds(120f); // 외부지원 호출에도 게임이 끝나지 않을 경우
            TriggerAnnihilation();
            
            
            bool IsEnd = false;
            while (!IsEnd)
            {
                if (juggernaut.IsAlive)
                {
                    List<Player> alivePlayers = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC).ToList();
                    List<Player> aliveJuggernautTeam = alivePlayers.Where(IsJuggernautTeam).ToList();

                    if (!alivePlayers.Except(aliveJuggernautTeam).Any()) {
                        PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, "<size=25>더 이상 저지할 수 있는 <b>Site-76 구성원</b>이 없습니다.</size>\n<color=#298A08>저거너트</color>의 승리입니다."));
                        IsEnd = true;
                        Timing.RunCoroutine(Tools.SetWinner(aliveJuggernautTeam, aliveJuggernautTeam.Count == 1 ? 20 : 4));
                    }
                }
                else {
                    PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, "<size=25><color=#298A08>저거너트</color>가 사망했습니다.</size>\n<b>Site-76 구성원</b>들의 승리입니다."));
                    IsEnd = true;
                    Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x.IsAlive).ToList(), 2));
                }

                yield return Timing.WaitForSeconds(1f);
            }   

            PlayerManager.List.ToList().ForEach(x => x.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None));
            Round.IsLocked = false;
        }

        public IEnumerator<float> AutoWarhead()
        {
            yield return Timing.WaitForSeconds(11 * 60);

            if (Warhead.IsDetonated)
                yield break;

            Tools.MessageTranslated("", $"1분 뒤 <color=red>자동핵</color>이 작동됩니다.");

            if (Warhead.IsDetonated)
                yield break;

            yield return Timing.WaitForSeconds(1 * 60);

            DeadmanSwitch.StartWarhead();
        }

        public IEnumerator<float> FindLocate()
        {
            while (!Round.IsEnded)
            {
                if (Tools.TryGetNearestPlayer(juggernaut, out _, out var radius))
                {
                    int alivePlayersCount = PlayerManager.List.Count(x => x.IsAlive && !x.IsNPC && !IsJuggernautTeam(x));
                    juggernaut.AddHint("저거너트 레이더", $"<b>[ 남은 인원 : {alivePlayersCount}명 | 가장 가까운 유기체와의 거리 : {radius:F1}m ]</b>", 1.1f);
                }
                else
                    juggernaut.AddHint("저거너트 승리", "당신은 임무를 완수하였습니다.", 1.1f);

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> MusicAsync()
        {
            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player {juggernaut.DisplayNickname}", condition: (ReferenceHub hub) =>
            {
                return !MuteBGMPlayers.Contains(Player.Get(hub));
            }, onIntialCreation: (p) =>
            {
                p.transform.parent = juggernaut.GameObject.transform;

                Speaker speaker = p.AddSpeaker("Main", isSpatial: true, minDistance: 5f, maxDistance: 20f);

                speaker.transform.parent = juggernaut.GameObject.transform;

                speaker.transform.localPosition = Vector3.zero;
            });

            audioPlayer.TryPlay("JuggernautTheme", loop: true);

            yield return 0;
        }
        public IEnumerator<float> ReduceWaveTimer()
        {
            while (true)
            {
                foreach (var wave in WaveManager.Waves)
                {
                    bool flag = WaveTimer.TryGetWaveTimers(wave.TargetFaction, out List<WaveTimer> waves);

                    foreach (var w in waves)
                        w.SetTime((int)w.TimeLeft.TotalSeconds - 5);
                }

                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void OnRespawnedTeam(RespawnedTeamEventArgs ev)
        {
            if (!juggernautExternalSupportWaves.Remove(ev.Wave))
                return;

            EventArgs.ServerEvents.CallTutorialSupport(ev.Players);
        }

        public void Spawned(Player player)
        {
            if (player.IsAlive && player.IsScpRole())
            {
                List<RoleTypeId> ScpsList = new List<RoleTypeId>()
                {
                    RoleTypeId.Scp3114,
                    RoleTypeId.Scp079,
                    RoleTypeId.Scp049
                };

                if (ScpsList.Contains(player.Role)) {
                    player.Role.Set(Tools.GetRandomValue(Tools.EnumToList<RoleTypeId>() 
                        .Where(x => !ScpsList.Contains(x) && x.IsScpRole()).ToList()));
                }
            }
        }

        public void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.IsAllowed = false;
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.IsAllowed = false;
        }

        public void OnShooting(ShootingEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.Player.CurrentItem.As<Firearm>().MagazineAmmo = 250;
        }

        public IEnumerator<float> OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                var isPlayerJuggernautTeam = IsJuggernautTeam(ev.Player);
                var isAttackerJuggernautTeam = IsJuggernautTeam(ev.Attacker);

                if (isPlayerJuggernautTeam == isAttackerJuggernautTeam)
                {
                    ev.IsAllowed = false;
                    yield break;
                }

                if (isPlayerJuggernautTeam || isAttackerJuggernautTeam)
                {
                    if (ev.Attacker == juggernaut && ev.Player != juggernaut)
                    {
                        if (ev.DamageHandler.CustomBase is FirearmDamageHandler { Hitbox: HitboxType.Headshot } damageHandler)
                            damageHandler.Damage /= 2;

                        ev.DamageHandler.Damage *= 3.35f;
                    }
                    else if (ev.Player == juggernaut)
                    {
                        if (!PlayerDamages.ContainsKey(ev.Attacker))
                            PlayerDamages.Add(ev.Attacker, 0);

                        PlayerDamages[ev.Attacker] += ev.DamageHandler.Damage;
                        stack += ev.DamageHandler.Damage;

                        if (stack > 300) {
                            Respawn.GrantInfluence(Faction.FoundationStaff, 20);
                            Respawn.GrantInfluence(Faction.FoundationEnemy, 20);
                                  
                            if (stack > 3000) {
                                CallRegularSupport();
                                stack = 0;
                            }
                        }

                        foreach (var wave in WaveManager.Waves)
                        {
                            WaveTimer.TryGetWaveTimers(wave.TargetFaction, out List<WaveTimer> waves);

                            foreach (var w in waves)
                                w.SetTime((int)w.TimeLeft.TotalSeconds - 5);
                        }

                        List<RoleTypeId> Scps = new List<RoleTypeId>() 
                        { 
                            RoleTypeId.Scp173,
                            RoleTypeId.Scp106,
                            RoleTypeId.Scp096,
                            RoleTypeId.Scp939,
                            RoleTypeId.Scp0492
                        };

                        if (ev.IsInstantKill || (Scps.Contains(ev.Attacker.Role.Type) && !ScpAttackCooldown.Contains(ev.Attacker)))
                        {
                            ev.IsAllowed = false;
                            ev.Player.Hurt(200f, DamageType.Scp);
                            ev.Attacker.ShowHitMarker(1.5f);

                            ScpAttackCooldown.Add(ev.Attacker);

                            yield return Timing.WaitForSeconds(1.1f);

                            ScpAttackCooldown.Remove(ev.Attacker);
                        }
                    }
                } 
                else 
                {
                    ev.IsAllowed = false;
                }
            }
        }

        public void OnReceivingEffect(ReceivingEffectEventArgs ev)
        {
            if (ev.Player == juggernaut && ev.Effect.GetEffectType() != EffectType.SinkHole)
            {
                if (ev.Effect.GetEffectType() == EffectType.PocketCorroding) ev.IsAllowed = false;

                else if (ev.Effect.GetEffectType().GetCategories() == EffectCategory.Harmful ||
                         ev.Effect.GetEffectType().GetCategories() == EffectCategory.Negative)
                {
                    if (Warhead.IsDetonated) return;
                    
                    Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                    {
                        ev.Player.DisableEffect(ev.Effect.GetEffectType());
                    });
                }
            }
        }

        public void OnHandcuffing(HandcuffingEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnChargingJailbird(Exiled.Events.EventArgs.Item.ChargingJailbirdEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.Item.As<Jailbird>().TotalCharges = 0;
        }
    }
}
