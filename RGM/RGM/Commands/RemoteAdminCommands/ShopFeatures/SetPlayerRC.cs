using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;

using PlayerRoles;
using RGM.API;
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

namespace RGM.Commands.RemoteAdminCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SetPlayerRC : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string userId = Tools.TryGetUserId(arguments.At(0));
            bool result = int.TryParse(arguments.Count() < 2 ? "dum" : arguments.At(1), out int rc);
            List<string> uc = UsersManager.UsersCache[userId];

            bool flag = userId.SetRC(rc, out response, result);
            return flag;
        }

        public string Command { get; } = "rc";

        public string[] Aliases { get; } = { "알피", "랜덤코인" };

        public string Description { get; } = "특정 유저의 랜덤코인을 정합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
