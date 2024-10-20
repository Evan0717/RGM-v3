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
                if (CurrentMode == null)
                {
                    response = "현재 모드가 설정되지 않았습니다.";
                    return false;
                }
                else
                {
                    string ModeColor = ModeList[CurrentMode][0];
                    string ModeDescription = ModeList[CurrentMode][1];
                    string ModeFileName = ModeList[CurrentMode][2];
                    string ModeDescriptionDetail = ModeList[CurrentMode][5];

                    string Message = Notions.StartModeDescription
                        .Replace("{ModeColor}", ModeColor)
                        .Replace("{CurrentMode}", CurrentMode)
                        .Replace("{CurrentSubMode}", CurrentSubMode != null ? $"<size=20>추가된 서브 모드 : <color=#{ModeList[CurrentSubMode][0]}>{CurrentSubMode}</color></size>\n" : "")
                        .Replace("{ModeDescription}", ModeDescription);

                    response = $"성공적으로 모드 설명을 불러왔습니다.";

                    player.SendConsoleMessage($"\n{Message}", "white");
                    if (ModeDescriptionDetail == "")
                        player.SendConsoleMessage($"\n해당 모드에 대한 자세한 설명이 없습니다.", "white");

                    else
                        player.SendConsoleMessage($"\n{ModeDescriptionDetail}", "white");

                    return true;
                }
            }
            else
            {
                string args = string.Join(" ", arguments).Trim();

                if (ModeList.Keys.Contains(args))
                {
                    CurrentMode = args;

                    string ModeColor = ModeList[CurrentMode][0];
                    string ModeDescription = ModeList[CurrentMode][1];
                    string ModeFileName = ModeList[CurrentMode][2];
                    string ModeDescriptionDetail = ModeList[CurrentMode][5];

                    string Message = Notions.StartModeDescription
                        .Replace("{ModeColor}", ModeColor)
                        .Replace("{CurrentMode}", CurrentMode)
                        .Replace("{CurrentSubMode}", CurrentSubMode != null ? $"<size=20>추가된 서브 모드 : <color=#{ModeList[CurrentSubMode][0]}>{CurrentSubMode}</color></size>\n" : "")
                        .Replace("{ModeDescription}", ModeDescription);

                    response = $"성공적으로 모드 설명을 불러왔습니다.";

                    player.SendConsoleMessage($"\n{Message}", "white");
                    if (ModeDescriptionDetail == "")
                        player.SendConsoleMessage($"\n해당 모드에 대한 자세한 설명이 없습니다.", "white");

                    else
                        player.SendConsoleMessage($"\n{ModeDescriptionDetail}", "white");

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
