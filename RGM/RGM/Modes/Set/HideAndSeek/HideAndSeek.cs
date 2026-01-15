using AFK;
using CustomPlayerEffects;
using CustomRendering;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using Mirror;
using MultiBroadcast;
using MultiBroadcast.API;
using PlayerRoles;
using Respawning;
using RGM.API.Features;
using RGM.Modes.Abilities.Synergy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.HideAndSeek)]
    class HideAndSeek : Mode
    {
        public override string Name => "숨바꼭질";
        public override string Description => "술래를 피해 특정 구역에서 제한 시간동안 버티세요!";
        public override string Detail =>
"""
꼭꼭 숨어라~
""";
        public override string Color => "e7c77d";

        List<Player> finders = new List<Player>();

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.PauseWaves(); 
            AFKManager._kickTime = 120500;

            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot += OnShot;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot -= OnShot;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var door in Door.List)
            {
                if (new List<DoorType>
                {
                    DoorType.CheckpointGateA,
                    DoorType.CheckpointGateB,
                    DoorType.ElevatorLczA,
                    DoorType.ElevatorLczB
                }.Contains(door.Type))
                {
                    door.Lock(DoorLockType.AdminCommand);
                }
                else
                {
                    door.IsOpen = true;
                }
            }

            Exiled.API.Features.Map.CleanAllItems();

            for (float i = 1; i < PlayerManager.List.Count / 6 + 2; i++)
            {
                finders.Add(Tools.GetRandomValue(PlayerManager.List.Where(x => !finders.Contains(x)).ToList()));
            }

            foreach (var Finder in finders)
            {
                Finder.Role.Set(RoleTypeId.Tutorial);
                Finder.Role.Set(RoleTypeId.Scp049, RoleSpawnFlags.None);

                Server.ExecuteCommand($"/speak {Finder.Id} 1");
            }

            foreach (var player in PlayerManager.List.Where(x => !finders.Contains(x)))
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.Scale = new Vector3(0.4f, 0.4f, 0.4f);
                player.EnableEffect(EffectType.Lightweight, 100);
                player.Position = Door.Get(DoorType.HIDLab).Position + new Vector3(0, 2, 0);

                Server.ExecuteCommand($"/speak {player.Id} 1");
            }

            for (int i = 1; i < 60; i++)
            {
                MultiBroadcast.API.MultiBroadcast.AddMapBroadcast(1, $"<size=25><b><color=red>{60 - i}초 뒤 술래가 출몰합니다. 빨리 숨으세요!</color></b></size>");

                yield return Timing.WaitForSeconds(1f);
            }

            foreach (var player in PlayerManager.List)
            {
                Server.ExecuteCommand($"/speak {player.Id} 0");
            }

            int Remaining = 300;

            foreach (var player in PlayerManager.List.Where(x => !finders.Contains(x)))
            {
                player.Scale = new Vector3(0.4f, 0.4f, 0.4f);
                player.EnableEffect(EffectType.SinkHole);
                player.DisableEffect(EffectType.Lightweight);
                player.EnableEffect(EffectType.HeavyFooted, 100);
                player.AddItem(ItemType.KeycardChaosInsurgency);
            }

            foreach (var finder in finders)
            {
                finder.Role.Set(RoleTypeId.FacilityGuard);
                finder.Scale = new Vector3(0.4f, 0.4f, 0.4f);
                finder.ClearInventory();
                finder.EnableEffect(EffectType.Lightweight, 100);
                finder.EnableEffect(EffectType.MovementBoost, 30);
                foreach (var item in new List<ItemType>
                {
                    ItemType.Radio,
                    ItemType.GunLogicer,
                })
                {
                    finder.AddItem(item);
                }
                finder.Position = Door.Get(DoorType.HIDLab).Position + new Vector3(0, 2, 0);
            }

            yield return Timing.WaitForSeconds(1f);

            for (int i = 1; i < Remaining; i++)
            {
                MultiBroadcast.API.MultiBroadcast.AddMapBroadcast(1, $"<size=25><b><color=#2EFEF7>{Remaining - i}초 뒤 술래가 패배합니다.</color></b></size>");

                if (i == 275)
                {
                    foreach (var finder in finders)
                    {
                        finder.AddEffect(EffectType.Scp1344, 1);
                    }

                    MultiBroadcast.API.MultiBroadcast.AddMapBroadcast(10, $"<size=25>모든 술래에게 <color=red>SCP-1344</color>가 지급됩니다, 행운을 빕니다!</size>");
                }

                yield return Timing.WaitForSeconds(1f);
            }

            Round.IsLocked = false;

            foreach (var player in Player.List.Where(x => x.Role.Type == RoleTypeId.FacilityGuard))
            {
                player.Kill($"제한 시간 안에 생존자를 전부 죽이지 못했습니다.");
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

            if (players.Count() == 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

            else if (players.Count() > 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
        }

        void OnDroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        void OnDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        void OnShot(ShotEventArgs ev)
        {
            ev.Player.AddAmmo(ev.Firearm.AmmoType, 1);
        }
    }
}
