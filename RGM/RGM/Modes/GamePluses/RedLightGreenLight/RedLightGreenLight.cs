using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Mirror;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    class RedLightGreenLight
    {
        public static RedLightGreenLight Instance;

        public string Light = "Green";
        public Dictionary<Player, Vector3> PlayerPosition = new Dictionary<Player, Vector3>();
        public Dictionary<Player, Quaternion> PlayerRotation = new Dictionary<Player, Quaternion>();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Timing.RunCoroutine(RecordPlayerInfo());
            Timing.RunCoroutine(CheckRedLight());

            Player.List.ToList().ForEach(x => x.ShowHint($"<color=green>초록 불</color>! 움직여도 됩니다.", 250));

            while (true)
            {
                yield return Timing.WaitForSeconds(UnityEngine.Random.Range(1, 51));

                Light = "Red";
                Player.List.ToList().ForEach(x => x.ShowHint($"<color=red>빨간 불</color>! 움직이지 마세요!", 250));

                yield return Timing.WaitForSeconds(UnityEngine.Random.Range(1, 6));

                Light = "Green";
                Player.List.ToList().ForEach(x => x.ShowHint($"<color=green>초록 불</color>! 움직여도 됩니다.", 250));
            }
        }

        public IEnumerator<float> RecordPlayerInfo()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (PlayerPosition.ContainsKey(player))
                    {
                        PlayerPosition[player] = player.Position;
                        PlayerRotation[player] = player.Rotation;
                    }
                    else
                    {
                        PlayerPosition.Add(player, player.Position);
                        PlayerRotation.Add(player, player.Rotation);
                    }

                    if (player.Role.Type == RoleTypeId.Scp079 && Player.List.Where(x => x.IsScp).Count() < 2)
                    {
                        player.Role.Set(RoleTypeId.Tutorial);
                        player.Position = Tools.GetRandomValue(Player.List.Where(x => x.IsHuman).ToList()).Position;
                        Server.ExecuteCommand($"/rocket {player.Id} 1");
                    }
                }

                yield return Timing.WaitForSeconds(0.49f);
            }
        }

        public IEnumerator<float> CheckRedLight()
        {
            while (true)
            {
                while (Light == "Green")
                    yield return Timing.WaitForSeconds(0.1f);

                yield return Timing.WaitForSeconds(1f);

                while (Light == "Red")
                {
                    foreach (var player in Player.List)
                    {
                        if (PlayerPosition.ContainsKey(player) && player.IsAlive)
                        {
                            if (PlayerPosition[player] != player.Position || PlayerRotation[player] != player.Rotation)
                            {
                                Server.ExecuteCommand($"/rocket {player.Id} 0.01");

                                Server.ExecuteCommand($"/clearcassie");
                                Server.ExecuteCommand($"/cassie_sl 안타깝게도 {player.DisplayNickname}(이)가 <b><color=red>빨간 불</color></b>에 건넜습니다.");
                            }
                        }
                    }

                    yield return Timing.WaitForSeconds(0.1f);
                }
            }
        }
    }
}
