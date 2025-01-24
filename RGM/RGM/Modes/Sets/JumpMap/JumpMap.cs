using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using JumpMap;
using MEC;
using Mirror;
using MultiBroadcast.API;
using PlayerRoles.FirstPersonControl;
using Respawning;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Private, ModeInfo.Set, ModeType.JumpMap)]
    class JumpMapLounge : Mode
    {
        public override string Name => "점프맵 라운지";
        public override string Description => "5분 안에 최대한 멀리 가세요!";
        public override string Detail =>
"""
스테이지가 총 11라운드로 이루어져 있습니다.

7 스테이지의 경우에는 고수만 해법을 찾을 수 있습니다.
되도록이면 중앙으로 가거나, 넉백되어도 괜찮을 만큼 여유를 두십시오.
""";
        public override string Color => "A9D0F5";

        public static JumpMapLounge Instance;

        JumpMap.JumpMap site02 = new JumpMap.JumpMap();

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.PauseWaves();

            Timing.RunCoroutine(OnModeStarted());

            site02.OnEnabled();
            site02.OnRoundStarted();

            foreach (var player in Player.List)
                site02.Verified(player);
        }

        public IEnumerator<float> OnModeStarted()
        {
            Tools.TryInstallMode(ModeType.FriendlyFire);

            for (int i = 0; i < 300; i++)
            {
                Player.List.ToList().ForEach(x => x.AddBroadcast(1, $"<b><size=25><color=green>{300 - i}초 후</color> 게임이 종료됩니다.</size></b>"));

                yield return Timing.WaitForSeconds(1f);
            }

            Player first = Player.List.OrderByDescending(x => int.Parse(site02.Stage[x.UserId])).ToList()[0];
            List<Player> farthestPlayers = Player.List.Where(x => int.Parse(site02.Stage[x.UserId]) == int.Parse(site02.Stage[first.UserId])).ToList();
            string playerNames = string.Join(", ", farthestPlayers.Select(x => $"<color=#ffd700>{x.DisplayNickname}</color>(Stage {site02.Stage[x.UserId]})"));

            Player.List.ToList().ForEach(x => x.AddBroadcast(15, $"<b><size=35>가장 멀리 간 유저는 {playerNames}입니다!</size></b>"));

            if (farthestPlayers.Count() == 1)
                Timing.RunCoroutine(Tools.SetWinner(farthestPlayers.ToList(), 5));

            else if (farthestPlayers.Count() > 1)
                Timing.RunCoroutine(Tools.SetWinner(farthestPlayers.ToList(), 1));

            Round.IsLocked = false;
        }
    }
}
