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
    public class SetPlayerRC : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string player = Tools.TryGetUserId(arguments.At(0));
            bool result = int.TryParse(arguments.Count() < 2 ? "dum" : arguments.At(1), out int rc);
            List<string> uc = UsersManager.UsersCache.ContainsKey(player) ? UsersManager.UsersCache[player] : new List<string>();

            if (uc.Count == 0)
            {
                List<string> DefaultValues = Enumerable.Repeat("0", 15).ToList();

                if (!UsersManager.UsersCache.ContainsKey(player))
                {
                    UsersManager.AddUser(player, DefaultValues);

                    UsersManager.SaveUsers();
                }

                uc = UsersManager.UsersCache[player];
            }

            if (result)
            {
                if (rc < 0)
                {
                    response = "0 upper.";
                    return false;
                }
                else
                {
                    uc[1] = rc.ToString();
                    UsersManager.UsersCache[player] = uc;
                    response = "successfully set up Random Coin.";

                    UsersManager.SaveUsers();
                    return true;
                }
            }
            else
            {
                response = $"{uc[1]}";
                return false;
            }
        }

        public string Command { get; } = "rc";

        public string[] Aliases { get; } = { "알피" };

        public string Description { get; } = "특정 유저의 랜덤코인을 정합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
