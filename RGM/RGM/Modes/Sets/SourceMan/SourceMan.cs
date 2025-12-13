using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.API.Features.Items;
using MEC;
using Mirror;
using MultiBroadcast;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API.Features;
using RGM.UserSettings;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Private, ModeInfo.Set, ModeType.SourceMan)]
    class SourceMan : Mode
    {
        public override string Name => "소스맨";
        public override string Description => "유저 중 하나가 절대 권력을 가진 <color=red>소스맨</color>이 됩니다.";
        public override string Detail =>
"""
일반 라운드 게임 형식으로 진행되는데, 절대 권력자가 한명 추가되는 구조입니다.
소스맨으로 발탁된 유저는 주어진 권한 하에서 모든 행동을 자유로이 수행할 수 있습니다.
단, 서버를 터트릴 정도의 행동은 금지되어 있습니다.

소스맨이 되고 싶다면, Server-specific 설정에서 참가 버튼을 누르세요.
제한 시간은 처음 3분이 주어지며, 유저들이 시간 단축, 시간 증가 중에서 선택할 수 있습니다.
""";
        public override string Color => "FFFFFF";

        public static SourceMan Instance;

        string Header = "<b>소스맨</b>";
        int time = 300;
        Player sourceman;

        List<Player> WantToBeSourceMan = new List<Player>();
        List<Player> TimeIncrease = new List<Player>();
        List<Player> TimeDecrease = new List<Player>();

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.PauseWaves();
            Map.IsDecontaminationEnabled = false;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            for (int i = 0; i < 30; i++)
            {
                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(1, $"<size=25><color=red>소스맨</color> 발탁이 진행중입니다. 관리자 권한에 대해 잘 알고 계신다면 참가하세요! {30 - i}초 남았습니다.</size>");
                }

                yield return Timing.WaitForSeconds(1);
            }

            if (WantToBeSourceMan.Count() == 0)
            {
                foreach (var player in PlayerManager.List)
                {
                    Server.ExecuteCommand($"/setgroup {player.Id} sourceman");

                    player.AddBroadcast(10, $"<size=30><color=red>소스맨</color> 지원을 아무도 하지 않았기 때문에 모두가 소스맨이 됩니다!</size>");
                }
            }
            else
            {
                sourceman = WantToBeSourceMan.GetRandomValue();

                Server.ExecuteCommand($"/setgroup {sourceman.Id} sourceman");

                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(10, $"<size=30>{sourceman.DisplayNickname}(이)가 이번 라운드의 <color=red>소스맨</color>입니다!</size>");
                }
            }

            while (time > 0)
            {
                foreach (var player in PlayerManager.List)
                {
                    player.AddBroadcast(1, $"<size=20>모드 종료까지 {time}초 남았습니다.</size>");
                }

                time--;

                yield return Timing.WaitForSeconds(1);
            }

            Round.IsLocked = false;

            foreach (var player in PlayerManager.List)
            {
                player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
            }
        }
    }
}
