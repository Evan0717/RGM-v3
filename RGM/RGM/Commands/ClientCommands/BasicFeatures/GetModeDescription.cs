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
using RGM.API.Components;
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

using static RGM.Variables.ServerManagers;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class GetModeDescription : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (arguments.Count == 0)
            {
                if (CurrentMode == ModeType.None)
                {
                    response = "현재 모드가 설정되지 않았습니다.";
                    return false;
                }
                else
                {
                    List<string> ModeDesc = Tools.GetModeDesc(CurrentMode, CurrentSubMode);

                    response = ModeDesc[1];

                    player.SendConsoleMessage(ModeDesc[0], "white");
                    if (ModeDesc[3] == "")
                        player.SendConsoleMessage($"\n{ModeDesc[2]}", "white");

                    else
                        player.SendConsoleMessage($"\n{ModeDesc[3]}", "white");

                    return true;
                }
            }
            else
            {
                string args = string.Join(" ", arguments).Trim();
                ModeData modeData = ModeList.Keys.FirstOrDefault(x => x.GetModeData().Name == args).GetModeData();

                if (ModeList.Keys.Select(x => x.GetModeData().Name).Contains(args))
                {
                    List<string> ModeDesc = Tools.GetModeDesc(modeData.Type, ModeType.None);

                    response = ModeDesc[1];

                    player.SendConsoleMessage(ModeDesc[0], "white");
                    if (ModeDesc[3] == "")
                        player.SendConsoleMessage($"\n{ModeDesc[2]}", "white");

                    else
                        player.SendConsoleMessage($"\n{ModeDesc[3]}", "white");

                    return true;
                }
                else
                {
                    response = "존재하지 않는 <모드 이름>입니다.\nSending Command Error..";
                    return false;
                }
            }
        }

        public string Command { get; } = "currentmode";

        public string[] Aliases { get; } = { "모드", "mode", "mod" };

        public string Description { get; } = "[RGM] 현재 모드를 확인합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
