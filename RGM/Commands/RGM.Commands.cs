using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using MEC;
using PluginAPI.Loader.Features;

namespace RGM.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceMode : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            string args = string.Join(" ", arguments).Trim();

            if (args == "")
            {
                List<string> ModeList = new List<string>();

                foreach (var Mode in RGM.Instance.ModeList.Keys)
                    ModeList.Add($"{Mode}");

                response = $"<b><size=30>[ 모드 리스트 ]</b></size>\n{string.Join(", ", ModeList)}\nSending Command Error..";
                return false;
            }
            else if (RGM.Instance.ModeList.Keys.Contains(args))
            {
                RGM.Instance.CurrentMode = args;
                Server.ExecuteCommand($"/cassie_sl <mark=#ffff00aa><color=#000000>관리자(<color=#ffffff>{player.Nickname}</color>)에 의하여 이번 라운드의 모드가 <b>{args}</b>으로 확정되었습니다.</color></mark>");
                response = $"이번 라운드의 모드는 <b>{args}</b>입니다.\nSending Command Complete!";
                return true;
            }
            else
            {
                response = "존재하지 않는 <모드 이름>입니다.\nSending Command Error..";
                return false;
            }

        }

        public string Command { get; } = "forcemode";

        public string[] Aliases { get; } = { "fm" };

        public string Description { get; } = "'/fm <모드 이름>'ㅣ현재 라운드의 모드를 강제합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Test : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "테스트 성공입니다. 제대로 작동하네요.\nHello, World!";

            return true;
        }

        public string Command { get; } = "test";

        public string[] Aliases { get; } = null;

        public string Description { get; } = "테스트용 명령어";

        public bool SanitizeResponse { get; } = true;
    }
}
