using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API;
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

using static RGM.Variables.ServerManagers;

namespace RGM.Modes
{
    public class VoteFirst : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Round.IsStarted)
            {
                Player player = Player.Get(sender);

                Requests.Add($"ABattle/{player.Id}/Vote/1");

                response = "1번 능력 선택 완료!";

                return true;
            }
            else
            {
                response = "라운드 시작 전에는 사용할 수 없습니다.";

                return false;
            }
        }

        public string Command { get; } = "1";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "워크스테이션 업그레이드ㅣ1번 능력 선택";

        public bool SanitizeResponse { get; } = true;
    }

    public class VoteSecond : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Round.IsStarted)
            {
                Player player = Player.Get(sender);

                Requests.Add($"ABattle/{player.Id}/Vote/2");

                response = "2번 능력 선택 완료!";

                return true;
            }
            else
            {
                response = "라운드 시작 전에는 사용할 수 없습니다.";

                return false;
            }
        }

        public string Command { get; } = "2";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "워크스테이션 업그레이드ㅣ2번 능력 선택";

        public bool SanitizeResponse { get; } = true;
    }

    public class VoteThird : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Round.IsStarted)
            {
                Player player = Player.Get(sender);

                Requests.Add($"ABattle/{player.Id}/Vote/3");

                response = "3번 능력 선택 완료!";

                return true;
            }
            else
            {
                response = "라운드 시작 전에는 사용할 수 없습니다.";

                return false;
            }
        }

        public string Command { get; } = "3";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "워크스테이션 업그레이드ㅣ3번 능력 선택";

        public bool SanitizeResponse { get; } = true;
    }
}
