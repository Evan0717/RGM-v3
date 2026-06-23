using Exiled.API.Features.DamageHandlers;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using System.Linq;
using Achievements.Handlers;
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
using LabApi.Features.Wrappers;
using Door = Exiled.API.Features.Doors.Door;
using Player = Exiled.API.Features.Player;
using Round = Exiled.API.Features.Round;
using Server = Exiled.API.Features.Server;
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
""";
        public override string Color => "088A08";

        public static Juggernaut Instance;

        Player juggernaut;
        List<Player> ScpAttackCooldown = new List<Player>();
        Dictionary<Player, float> PlayerDamages = new Dictionary<Player, float>();
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

            Exiled.Events.Handlers.Item.ChargingJailbird -= OnChargingJailbird;

            Timing.KillCoroutines(_onModeStarted);
            Timing.KillCoroutines(_autoWarhead);
            Timing.KillCoroutines(_findLocate);
            Timing.KillCoroutines(_musicAsync);
            Timing.KillCoroutines(_reduceWaveTimer);

            if (speaker != null)
                speaker.Destroy();
        }

        public IEnumerator<float> Timer()
        {
            for (int i = 1; i < 75; i++)
            {
                if (Round.IsEnded)
                    yield break;

                PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(1, $"<size=25>저거너트 외부 지원 호출까지 {75 - i}초</size>"));

                yield return Timing.WaitForSeconds(1f);
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
            juggernaut.MaxHealth = 530 * PlayerManager.List.Count();
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

            if (Warhead.IsDetonated) {
                juggernaut.DisableAllEffects();
                juggernaut.EnableEffect(EffectType.BodyshotReduction, 4);
                juggernaut.EnableEffect(EffectType.Scp1344, 1);
                juggernaut.EnableEffect(EffectType.Scp1853, 5);
                juggernaut.EnableEffect(EffectType.Scp207, 1);
                juggernaut.EnableEffect(EffectType.MovementBoost, 10);
                juggernaut.EnableEffect(EffectType.FogControl, 1);
                juggernaut.EnableEffect(EffectType.Bleeding, 1);
                PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(10, "<b><size=30><color=#298A08>저거너트</color>가 과부하 상태에 돌입합니다!</size></b>\n<size=25>모든 능력치가 강화되는 대신, 중독과 출혈 효과를 받습니다.</size>"));
                yield return Timing.WaitForSeconds(10f);
                
                // 저거너트 외부 지원 호출 구현(2번에 나눠서)
                Timing.RunCoroutine(Timer());
                yield return Timing.WaitForSeconds(75f); // 1차 호출
                /*
                 * NTFMiniWave 또는 ChaosMiniWave 중에서 랜덤으로 Instant Respawn 하고, 이를 튜토리얼로 변경
                 */
                Timing.RunCoroutine(Timer());
                yield return Timing.WaitForSeconds(75f); // 2차 호출
                /*
                 * NTFMiniWave 또는 ChaosMiniWave 중에서 랜덤으로 Instant Respawn 하고, 이를 튜토리얼로 변경
                 */
                
                // 초토화 작전 구현
                yield return Timing.WaitForSeconds(120f); // 외부지원 호출에도 게임이 끝나지 않을 경우
                
            }
            
            bool IsEnd = false;
            while (!IsEnd)
            {
                if (juggernaut.IsAlive)
                {
                    if (PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC).Count() < 2) {
                        PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, "<size=25>더 이상 저지할 수 있는 <b>Site-76 구성원</b>이 없습니다.</size>\n<color=#298A08>저거너트</color>의 승리입니다."));
                        IsEnd = true;
                        Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x.IsAlive).ToList(), 11));
                        /*
                         * 저거너트 외부 지원 호출 기능을 추가할 것이기 때문에, 기존의 검사 방식을 사용하면 안 됨.
                         * 이제 저거너트랑 튜토리얼을 같은 팀으로 취급하기 때문에, 이를 검사하는 방식으로 구현하되,
                         * 저거너트 단 하나만 생존할 경우 기존의 시스템을 그대로 사용.
                         *
                         * 저거너트만 살아있는가?
                         * 저거너트랑 튜토리얼이 같이 살았는가?
                         * 를 봐야 함.
                         */
                    }
                }
                else {
                    PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, "<size=25><color=#298A08>저거너트</color>가 사망했습니다.</size>\n<b>Site-76 구성원</b>들의 승리입니다."));
                    IsEnd = true;
                    Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x.IsAlive).ToList(), 1));
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
                if (Tools.TryGetNearestPlayer(juggernaut, out Player nearestPlayer, out float radius))
                    juggernaut.AddHint("저거너트 레이더", $"<b>[ <color={nearestPlayer.Role.Color.ToHex()}>{( Trans.Role[nearestPlayer.Role.Type])}</color>, 거리: {radius.ToString("F1")}m ]</b>", 1.2f);

                else
                    juggernaut.AddHint("저거너트 승리", "당신은 임무를 완수하였습니다.", 1.2f);

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
                        w.SetTime((int)w.TimeLeft.TotalSeconds - 3);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
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
                if (ev.Player == juggernaut || ev.Attacker == juggernaut)
                {
                    if (ev.Attacker == juggernaut && ev.Player != juggernaut)
                    {
                        if (ev.DamageHandler.CustomBase is FirearmDamageHandler { Hitbox: HitboxType.Headshot } damageHandler)
                            damageHandler.Damage /= 2;

                        ev.DamageHandler.Damage *= 3.30f;
                    }
                    else if (ev.Attacker != juggernaut && ev.Player == juggernaut)
                    {
                        if (!PlayerDamages.ContainsKey(ev.Attacker))
                            PlayerDamages.Add(ev.Attacker, 0);
                        /*
                         * RoleId가 Tutorial인 경우 juggernaut랑 서로 데미지를 입지 않아야 함.
                         * (두 진영을 같은 팀으로 만들어야 하기 때문)
                         */
                        PlayerDamages[ev.Attacker] += ev.DamageHandler.Damage;
                        stack += ev.DamageHandler.Damage;

                        while (true) {
                            if (stack > 300) {
                                Respawn.GrantInfluence(Faction.FoundationStaff, 20);
                                Respawn.GrantInfluence(Faction.FoundationEnemy, 20);
                                      
                                if (stack > 3000) {
                                    Respawn.GrantTokens(Faction.FoundationStaff, 1);
                                    Respawn.GrantTokens(Faction.FoundationEnemy, 1);
                                    /*
                                     * stack이 3000이 넘을 경우, NTF 또는 CHAOS 중에서 랜덤으로 지원을 즉시 호출
                                     * 단, Instant Respawn이 아닌 일반 Respawn으로 작동
                                     */
                                    stack = 0;
                                    break;
                                }

                            }
                        }

                        foreach (var wave in WaveManager.Waves)
                        {
                            bool flag = WaveTimer.TryGetWaveTimers(wave.TargetFaction, out List<WaveTimer> waves);

                            foreach (var w in waves)
                                w.SetTime((int)w.TimeLeft.TotalSeconds - 3);
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

                            yield return Timing.WaitForSeconds(1.2f);

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
                if (ev.Effect.GetEffectType() == EffectType.PocketCorroding)
                    ev.IsAllowed = false;

                else if (ev.Effect.GetEffectType().GetCategories() == EffectCategory.Negative)
                {
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
