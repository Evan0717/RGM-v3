using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MEC;
using Mirror;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.RedLightGreenLight)]
    class RedLightGreenLight : Mode
    {
        public override string Name => "빨간 불, 초록 불";
        public override string Description => "빨간 불에는 움직이지 마세요.";
        public override string Detail =>
"""
<i>순발력이 좋아야 살아남습니다.</i>

엘레베이터와 같은 움직이는 물체를 주의하세요!

아, 그리고 설마 죽을까봐 움직이지 않거나 조금씩만 이동하는 쫄보는 없겠죠?
""";
        public override string Color => "F7D358";

        public static RedLightGreenLight Instance;

        public string Light = "Green";
        public Dictionary<Player, Vector3> PlayerPosition = new Dictionary<Player, Vector3>();
        public Dictionary<Player, Quaternion> PlayerRotation = new Dictionary<Player, Quaternion>();

        public Quaternion rot(Player player)
        {
            if (player.Role is Scp079Role scp079role)
            {
                return scp079role.Camera.Rotation;
            }
            else
            {
                return player.CameraTransform.rotation;
            }
        }

        public override void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Timing.RunCoroutine(RecordPlayerInfo());
            Timing.RunCoroutine(CheckRedLight());

            Player.List.ToList().ForEach(x => x.AddHint("불", $"<color=green>초록 불</color>! 움직여도 됩니다.", 250));

            while (true)
            {
                yield return Timing.WaitForSeconds(UnityEngine.Random.Range(1, 51));

                Light = "Red";
                Player.List.ToList().ForEach(x => x.AddHint("불", $"<color=red>빨간 불</color>! 움직이지 마세요!", 250));

                yield return Timing.WaitForSeconds(UnityEngine.Random.Range(1, 6));

                Light = "Green";
                Player.List.ToList().ForEach(x => x.AddHint("불", $"<color=green>초록 불</color>! 움직여도 됩니다.", 250));
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
                        PlayerRotation[player] = rot(player);
                    }
                    else
                    {
                        PlayerPosition.Add(player, player.Position);
                        PlayerRotation.Add(player, rot(player));
                    }

                    if (player.Role.Type == RoleTypeId.Scp079 && Player.List.Where(x => x.IsScp).Count() < 2)
                    {
                        player.Role.Set(RoleTypeId.Tutorial);
                        player.Position = Tools.GetRandomValue(Player.List.Where(x => x.IsHuman).ToList()).Position;
                        Timing.RunCoroutine(Tools.DoRocket(player, player, 1f));
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
                            if (PlayerPosition[player] != player.Position || PlayerRotation[player] != rot(player))
                            {
                                if (player.Role.Type == RoleTypeId.Scp079)
                                {
                                    player.Role.Set(RoleTypeId.Tutorial);
                                    player.Position = Tools.GetRandomValue(Player.List.Where(x => x.IsHuman).ToList()).Position;
                                    Timing.RunCoroutine(Tools.DoRocket(player, player, 1f));
                                }
                                else
                                    Timing.RunCoroutine(Tools.DoRocket(player, player, 0.01f));

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
