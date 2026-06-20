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
using Exiled.API.Features.Doors;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.Juggernaut)]
    class Juggernaut : Mode
    {
        public override string Name => "저거너트";
        public override string Description => "모두 힘을 합쳐 외부의 적에 대항하세요.";
        public override string Detail =>
"""
지금은 우리(SCP, 반란, MTF)끼리 싸울 때가 아닙니다.
군대를 홀로 섬멸할 수 있는 전력인 저거너트가 재단을 점령하기 위해 왔습니다.

이제 친구가 될 시간이야.

* 게임 시작 10분 뒤 <color=red>자동핵</color>이 작동됩니다.
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
            juggernaut.MaxHealth = 350 * PlayerManager.List.Count() + 120 * PlayerManager.List.Count();
            juggernaut.Health = juggernaut.MaxHealth;
            juggernaut.IsBypassModeEnabled = true;
            juggernaut.EnableEffect(EffectType.SinkHole);
            juggernaut.EnableEffect(EffectType.DamageReduction, 10);
            juggernaut.AddBroadcast(10, "<b><size=30>당신은 <color=#298A08>저거너트</color>입니다.</size></b>\n<size=25>본인을 제외한 모두를 사살하십시오.</size>");
            juggernaut.Position = new Vector3(123.3271f, 288.7908f, 27.01838f);

            List<ItemType> Items = new List<ItemType>() { ItemType.GunLogicer, ItemType.Jailbird };
            foreach (var Item in Items)
                juggernaut.AddItem(Item);

            bool IsEnd = false;
            while (!IsEnd)
            {
                if (juggernaut.IsAlive)
                {
                    if (PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC).Count() < 2)
                    {
                        PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, "<size=25>더 이상 저지할 수 있는 <b>Site-76 구성원</b>이 없습니다.</size>\n<color=#298A08>저거너트</color>의 승리입니다."));
                        IsEnd = true;

                        Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x.IsAlive).ToList(), 5));
                    }
                }
                else
                {
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
            yield return Timing.WaitForSeconds(9 * 60);

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

                Speaker speaker = p.AddSpeaker("Main", isSpatial: true, minDistance: 5f, maxDistance: 15f);

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

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
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
                    RoleTypeId.Scp079
                };

                if (ScpsList.Contains(player.Role))
                    player.Role.Set(Tools.GetRandomValue(Tools.EnumToList<RoleTypeId>().Where(x => !ScpsList.Contains(x) && x.IsScpRole()).ToList()));
            }
        }

        public void OnSearchingPickup(Exiled.Events.EventArgs.Player.SearchingPickupEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.IsAllowed = false;
        }

        public void OnDroppingItem(Exiled.Events.EventArgs.Player.DroppingItemEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.IsAllowed = false;
        }

        public void OnShooting(Exiled.Events.EventArgs.Player.ShootingEventArgs ev)
        {
            if (ev.Player == juggernaut)
                ev.Player.CurrentItem.As<Firearm>().MagazineAmmo = 250;
        }

        public IEnumerator<float> OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (ev.Player == juggernaut || ev.Attacker == juggernaut)
                {
                    if (ev.Attacker == juggernaut && ev.Player != juggernaut)
                    {
                        ev.DamageHandler.Damage = ev.DamageHandler.Damage * 3;
                    }
                    else if (ev.Attacker != juggernaut && ev.Player == juggernaut)
                    {
                        if (!PlayerDamages.ContainsKey(ev.Attacker))
                            PlayerDamages.Add(ev.Attacker, 0);

                        PlayerDamages[ev.Attacker] += ev.DamageHandler.Damage;
                        stack += ev.DamageHandler.Damage;

                        if (stack > 150)
                        {
                            Respawn.GrantInfluence(Faction.FoundationStaff, 10);
                            Respawn.GrantInfluence(Faction.FoundationEnemy, 10);

                            stack = 0;
                        }

                        foreach (var wave in WaveManager.Waves)
                        {
                            bool flag = WaveTimer.TryGetWaveTimers(wave.TargetFaction, out List<WaveTimer> waves);

                            foreach (var w in waves)
                                w.SetTime((int)w.TimeLeft.TotalSeconds - 2);
                        }

                        List<RoleTypeId> Scps = new List<RoleTypeId>() 
                        { 
                            RoleTypeId.Scp173,
                            RoleTypeId.Scp049,
                            RoleTypeId.Scp106,
                            RoleTypeId.Scp096,
                            RoleTypeId.Scp939
                        };

                        if (ev.IsInstantKill || (Scps.Contains(ev.Attacker.Role.Type) && !ScpAttackCooldown.Contains(ev.Attacker)))
                        {
                            ev.IsAllowed = false;
                            ev.Player.Hurt(120.5f, DamageType.Scp);
                            ev.Attacker.ShowHitMarker(1.5f);

                            ScpAttackCooldown.Add(ev.Attacker);

                            yield return Timing.WaitForSeconds(1.5f);

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

        public void OnReceivingEffect(Exiled.Events.EventArgs.Player.ReceivingEffectEventArgs ev)
        {
            if (ev.Player == juggernaut && ev.Effect.GetEffectType() != EffectType.SinkHole)
            {
                if (ev.Effect.GetEffectType() == EffectType.PocketCorroding)
                    ev.IsAllowed = false;

                else
                {
                    Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                    {
                        ev.Player.DisableEffect(ev.Effect.GetEffectType());
                    });
                }
            }
        }

        public void OnHandcuffing(Exiled.Events.EventArgs.Player.HandcuffingEventArgs ev)
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
