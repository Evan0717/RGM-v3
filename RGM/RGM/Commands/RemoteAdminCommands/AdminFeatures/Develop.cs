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
    public class Develop : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            player.Kill("이 자는 개발의 의무를 짊어지고 죽었습니다.");
            player.Role.Set(RoleTypeId.Overwatch);

            response = "Complete!";

            return true;
        }

        public string Command { get; } = "develop";

        public string[] Aliases { get; } = { "dv", "dev", "개발", "roqkf" };

        public string Description { get; } = "개발하러 갈 때 사용하세요!";

        public bool SanitizeResponse { get; } = true;
    }
}
