using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;
using PlayerRoles;

using RGM.API.Features;
using Mirror;
using Respawning;
using Exiled.API.Features.Toys;

using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.DeadLine)]
    public class DeadLine : Mode
    {
        public override string Name => "데드 라인";
        public override string Description => "빨간색을 밟지 마세요!";
        public override string Detail =>
"""
<color=red>빨간색</color>을 밟으면 죽습니다.

<i>발 아래를 조심하세요!</i>
""";
        public override string Color => "FA8258";
        public override string Map => "dl";

        public static DeadLine Instance;

        public List<Player> pl = new List<Player>();

        CoroutineHandle _onModeStarted;

        AudioClipPlayback audio;

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.PauseWaves(); 
            Server.FriendlyFire = true;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Died += OnDied;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());

            audio = Tools.PlayGlobalAudio("LineLite", 1, true);
        }

        public override void OnDisabled()
        {
            Round.IsLocked = false;
            Respawn.ResumeWaves();
            Server.FriendlyFire = false;

            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Died -= OnDied;

            Timing.KillCoroutines(_onModeStarted);

            audio.IsPaused = true;
        }

        public IEnumerator<float> OnModeStarted()
        {
            PlayerManager.List.CopyTo(pl);

            foreach (var player in PlayerManager.List.Where(x => !x.IsNPC))
            {
                player.Role.Set(RoleTypeId.Scientist);
                player.Position = new Vector3(37.81419f, 340.06f, -51.64725f);
                player.ClearInventory();
            }

            while (true)
            {
                foreach (var player in PlayerManager.List.Where(x => !x.IsNPC && x.IsAlive))
                {
                    if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 1, (LayerMask)1))
                    {
                        if (hit.transform.name.Contains("Dead"))
                        {
                            if (pl.Contains(player))
                            {
                                pl.Remove(player);

                                if (pl.Count() < 2)
                                {
                                    Round.IsLocked = false;

                                    PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {pl[0].DisplayNickname}"));
                                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { pl[0] }, 5));
                                }

                                player.Kill("선을 넘어버렸다네~");
                            }
                        }
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Server.ExecuteCommand($"/speak {ev.Player.Id} 1");
            IntercomPlayers.Add(ev.Player);
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count() < 2)
                {
                    Round.IsLocked = false;

                    PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {pl[0].DisplayNickname}"));
                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { pl[0] }, 5));
                }
            }
        }
    }
}
