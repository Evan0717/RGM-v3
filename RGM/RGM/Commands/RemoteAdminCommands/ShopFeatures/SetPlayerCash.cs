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
    public class SetPlayerCash : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string player = Tools.TryGetUserId(arguments.At(0));
            bool result = int.TryParse(arguments.Count() < 2 ? "dum" : arguments.At(1), out int cash);
            List<string> uc = UsersManager.UsersCache.ContainsKey(player) ? UsersManager.UsersCache[player] : new List<string>();

            bool flag = Player.Get(sender).AddCash(cash, out response, result);
            return flag;
        }

        public string Command { get; } = "캐쉬";

        public string[] Aliases { get; } = { "cash" };

        public string Description { get; } = "특정 유저의 캐쉬를 정합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
