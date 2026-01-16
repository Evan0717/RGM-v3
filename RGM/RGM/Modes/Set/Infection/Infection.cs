using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomPlayerEffects;
using CustomRendering;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using ProjectMER.Features.Objects;
using MEC;
using Mirror;

using PlayerRoles;
using UnityEngine;
using Exiled.API.Enums;
using RGM.API.Features;

using static RGM.Variables.Variable;
using RGM.API.DataBases;
using Respawning;
using Exiled.Events.EventArgs.Server;
using Respawning.Waves;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Warhead;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.Infection)]
    class Infection : Mode
    {
        public override string Name => "감염";
        public override string Description => "모두를 감염시키려는 숙주와 살아남으려는 인류의 대립";
        public override string Detail =>
"""
<b>인류</b>는 무슨 수를 써서라도 10분을 버텨야 합니다.
<b><color=red>숙주</color></b>는 무슨 수를 써서라도 모든 인류를 감염시켜야 합니다.

* <b><color=red>숙주</color></b>는 일반 감염자보다 더 강력합니다.
* <color=red>SCP-079</color>는 <b><color=red>숙주</color></b>의 조력자로 탄생합니다.
""";
        public override string Color => "FF0000";

        public static Infection Instance;

        List<Player> HostZombies = new();
        bool IsHumanEnd = false;

        CoroutineHandle _onModeStarted;
        CoroutineHandle _checkEnd;

        AudioClipPlayback audio;

        public override void OnEnabled()
        {
            Respawn.PauseWaves();
            Round.IsLocked = true;

            Exiled.Events.Handlers.Warhead.Detonating += OnDetonating;

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.Died += OnDied;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _checkEnd = Timing.RunCoroutine(CheckEnd());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Warhead.Detonating -= OnDetonating;

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.Died -= OnDied;

            Timing.KillCoroutines(_onModeStarted);
            Timing.KillCoroutines(_checkEnd);

            audio.IsPaused = true;
        }

        public IEnumerator<float> OnModeStarted()
        {
            audio = Tools.PlayGlobalAudio("Voices", 0.3f, true);

            for (int i = 0; i < Mathf.Max(1, PlayerManager.List.Count() / 7); i++)
            {
                Player hostZombie = Tools.GetRandomValue(PlayerManager.List.Where(x => x.IsAlive && !HostZombies.Contains(x)).ToList());

                HostZombies.Add(hostZombie);

                Timing.CallDelayed(1, () =>
                {
                    hostZombie.Role.Set(RoleTypeId.Scp0492);
                });
            }

            foreach (var player in PlayerManager.List)
            {
                try
                {
                    if (!HostZombies.Contains(player))
                    {
                        player.Role.Set(RoleTypeId.NtfCaptain, RoleSpawnFlags.AssignInventory);
                        player.AddItem(ItemType.Ammo556x45, 10);

                        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                        {
                            foreach (var item in player.Items)
                            {
                                if (item.Type == ItemType.KeycardMTFCaptain)
                                    player.RemoveItem(item);
                            }
                        });
                        player.AddItem(ItemType.KeycardScientist);
                    }
                }
                catch (Exception ex)
                {
                    player.AddHint("에러", $"Error: {ex}");
                }
            }

            for (int i = 0; i < 500; i++)
            {
                MultiBroadcast.API.MultiBroadcast.AddMapBroadcast(1, $"<b><size=25>{500 - i}초 뒤 인류가 승리합니다.</size></b>");

                yield return Timing.WaitForSeconds(1);
            }

            if (!Round.IsEnded)
            {
                Round.IsLocked = false;
                IsHumanEnd = true;
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x.IsHuman).ToList(), PlayerManager.List.Where(x => x.IsHuman).Count() == 1 ? 10 : 1));

                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(20, $"<size=30><b>인류의 승리입니다. <color=#9AFE2E>좀비들은 해독제를 맞고 치료되었습니다.</color></b></size>");

                    if (player.Role.Type == RoleTypeId.Scp0492)
                        player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
                }
            }
        }

        public IEnumerator<float> CheckEnd()
        {
            while (!Round.IsEnded)
            {
                if (PlayerManager.List.Where(x => x.IsHuman).Count() < 1)
                {
                    Round.IsLocked = false;
                    Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x.Role.Type == RoleTypeId.Scp0492).ToList(), 1));

                    MultiBroadcast.API.MultiBroadcast.AddMapBroadcast(20, $"<size=30><b>숙주의 승리입니다. <color=red>남겨진 인류는 안타까운 결말을 맞이할 것입니다.</color></b></size>");

                    yield break;
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        void OnDetonating(DetonatingEventArgs ev)
        {
            Door door = Door.Get(DoorType.NukeSurface);

            foreach (var player in PlayerManager.List.Where(x => x.IsScpRole()))
            {
                player.Position = door.Position + new Vector3(0, 2, 0);
            }
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.Kill("동료들과 함께 인간을 섬멸하십시오.");
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                ev.Player.EnableEffect(EffectType.FogControl, 7);

                if (HostZombies.Contains(ev.Player))
                {
                    ev.Player.MaxHealth = 555;
                    ev.Player.Health = ev.Player.MaxHealth;
                    ev.Player.EnableEffect(EffectType.MovementBoost, 15);
                    ev.Player.IsBypassModeEnabled = true;
                }

                if (ev.Player.Role.Type == RoleTypeId.Scp079)
                {
                    if (!HostZombies.Contains(ev.Player))
                        HostZombies.Add(ev.Player);

                    if (GodModePlayers.Contains(ev.Player))
                        GodModePlayers.Remove(ev.Player);

                    ev.Player.Kill("숙주 좀비가 될 것입니다.");
                }
            });
        }

        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (HostZombies.Contains(ev.Player))
            {
                if (ev.Door is BreakableDoor door)
                {
                    door.Break();
                }
            }
        }

        public IEnumerator<float> OnDied(DiedEventArgs ev)
        {
            for (int i = 0; i < 9; i++)
            {
                ev.Player.ShowHint($"<size=25>{9 - i}초 뒤 <color=red>동료</color> 근처에서 부활합니다.</size>", 1.2f);

                yield return Timing.WaitForSeconds(1f);
            }

            if (!IsHumanEnd)
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492);
                ev.Player.MaxHealth = 455;
                ev.Player.Health = ev.Player.MaxHealth;

                try
                {
                    IEnumerable<Player> zombies = PlayerManager.List.Where(x => x.Role.Type == RoleTypeId.Scp0492);

                    if (zombies.Count() < 1)
                        ev.Player.Position = Tools.GetRandomValue(PlayerManager.List.Where(x => x.Role.Type == RoleTypeId.NtfCaptain).Select(x => x.Position).ToList());

                    else
                        ev.Player.Position = Tools.GetRandomValue(zombies.Select(x => x.Position).ToList());
                }
                catch (Exception ex)
                {
                    ev.Player.Position = Tools.GetRandomValue(PlayerManager.List.Where(x => x.Role.Type == RoleTypeId.NtfCaptain).Select(x => x.Position).ToList());
                }
            }
            else
                ev.Player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
        }
    }
}
