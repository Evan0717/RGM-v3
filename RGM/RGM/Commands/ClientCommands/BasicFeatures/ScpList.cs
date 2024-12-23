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

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ScpList : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player.IsScp)
            {
                response = "\n" + string.Join("\n", Player.List.Where(x => x.IsScp).Select(x => $"{x.Role.Name} : {x.Nickname}"));
                return true;
            }
            else
            {
                response = "SCP만 사용할 수 있는 명령어입니다.";
                return false;
            }
        }

        public string Command { get; } = "scp리스트";

        public string[] Aliases { get; } = { "sl", "scp", "scplist" };

        public string Description { get; } = "[RGM] 존재하는 SCP 리스트를 나열합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
