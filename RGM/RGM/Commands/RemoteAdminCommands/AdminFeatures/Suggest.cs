using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;

using PlayerRoles;
using RGM.API;
using RGM.API.Components;
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Suggest : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (IsSuggestProcessing)
            {
                response = "이미 의문의 제안이 진행중입니다.\nSuggest";
                return false;
            }
            else if (arguments.Count < 1)
            {
                response = "제안 내용을 기입해주세요.\nSuggest";
                return false;
            }
            else
            {
                IsSuggestProcessing = true;

                Timing.RunCoroutine(Tools.Suggest(player, string.Join(" ", arguments)));

                SuggestPlayers.Add(player);

                response = "의문의 제안을 성공적으로 개설하였습니다.\nSuggest";
                return true;
            }
        }

        public string Command { get; } = "제안";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "[RGM] 의문의 제안";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Accept : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!IsSuggestProcessing)
            {
                response = "의문의 제안이 진행중이 아닙니다.";
                return false;
            }
            else if (SuggestPlayers.Contains(player))
            {
                response = "이미 의문의 제안을 수락하였습니다.";
                return false;
            }
            else
            {
                SuggestPlayers.Add(player);

                response = "의문의 제안에 수락하였습니다.";
                return true;
            }
        }

        public string Command { get; } = "수락";

        public string[] Aliases { get; } = { "accept" };

        public string Description { get; } = "[RGM] 의문의 제안에서 수락할 때 사용하세요.";

        public bool SanitizeResponse { get; } = true;
    }
}
