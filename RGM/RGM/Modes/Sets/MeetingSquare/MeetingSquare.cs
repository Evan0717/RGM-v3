using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using Mirror;
using MultiBroadcast;
using MultiBroadcast.API;
using PlayerRoles;
using Respawning;
using RGM.API.Features;
using UnityEngine;
using Exiled.Loader;
using Exiled.API.Enums;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.MeetingSquare)]
    class MeetingSquare : Mode
    {
        public override string Name => "<맛보기> 만남의 광장";
        public override string Description => "만남의 광장에서 동전 4개를 가장 먼저 모으세요!";
        public override string Detail =>
"""
동전은 맵 곳곳에 스폰됩니다.
하지만 주의하세요, 유저들은 서로를 공격할 수 있습니다!

총 3개의 동전을 먼저 모은 유저가 우승합니다. 행운을 빌어요!
""";
        public override string Color => "F5D0A9";

        public static MiniGames Instance;

        public bool IsEnd = false;

        public override void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Round.IsLocked = true;

            AudioClipStorage.AudioClips.Clear();

            Castle.Main main = new Castle.Main();
            main.OnEnabled();

            Timing.RunCoroutine(Castle.Core.EventArgs.ServerEvents.OnWaitingForPlayers());

            foreach (var player in Player.List.Where(x => x.IsVerified))
            {
                player.Role.Set(RoleTypeId.Tutorial);

                Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                {
                    Timing.RunCoroutine(Castle.Core.EventArgs.PlayerEvents.Verified(player));
                });
            }

            while (!IsEnd)
            {
                foreach (var player in Player.List)
                {
                    if (player.GetEffect(EffectType.Blinded).Intensity > 0)
                        player.GetEffect(EffectType.Blinded).Intensity = 0;

                    if (player.Items.Where(x => x.Type == ItemType.Coin).Count() >= 3)
                    {
                        Timing.RunCoroutine(Tools.SetWinner(new List<Player> { player }, 7));

                        foreach (var p in Player.List)
                            p.Role.Set(RoleTypeId.Tutorial);

                        Round.IsLocked = false;
                        IsEnd = true;

                        yield break;
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}