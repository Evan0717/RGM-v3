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

namespace RGM.Commands.RemoteAdminCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceMode : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player.TryGet(sender, out Player player);

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

                if (player != null)
                    Server.ExecuteCommand($"/cassie_sl <mark=#ffff00aa><color=#000000>운영진(<color=#ffffff>{player.Nickname}</color>)에 의하여 이번 라운드의 모드가 <b>{args}</b>으로 확정되었습니다.</color></mark>");
                
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
    public class ForceModeReset : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            RGM.Instance.PickModes();
            response = "모드 리셋이 완료되었습니다.";
            return true;
        }

        public string Command { get; } = "forcemodereset";

        public string[] Aliases { get; } = { "fmr" };

        public string Description { get; } = "'/fmrㅣ모드 투표를 강제로 리셋합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StartMode : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string args = string.Join(" ", arguments).Trim();

            if (args == "")
            {
                List<string> ModeList = new List<string>();

                foreach (var Mode in RGM.Instance.ModeList.Keys)
                    ModeList.Add($"{Mode}");

                response = $"<b><size=30>[ 모드 리스트 ]</b></size>\n{string.Join(", ", ModeList)}\nSending Command Error..";
                return false;
            }
            else if (Tools.TryInstallMode(args))
            {
                response = $"모드 <b>{args}</b>(을)를 강제로 시작했습니다.\nSending Command Complete!";
                return true;
            }
            else
            {
                response = "존재하지 않는 <모드 이름>입니다.\nSending Command Error..";
                return false;
            }
        }

        public string Command { get; } = "startmode";

        public string[] Aliases { get; } = { "모드시작", "sm" };

        public string Description { get; } = "'/smㅣ모드를 강제로 시작합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
