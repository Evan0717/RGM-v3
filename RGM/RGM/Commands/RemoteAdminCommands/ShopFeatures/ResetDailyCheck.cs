using System;
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
    public class ResetDailyCheck : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            foreach (var steamId in UsersManager.UsersCache.Keys)
            {
                try
                {
                    UsersManager.UsersCache[steamId][29] = "0";
                }
                catch
                {
                }
            }

            UsersManager.SaveUsers();

            response = "모두의 출석 체크가 초기화되었습니다.";
            return true;
        }

        public string Command { get; } = "출석체크초기화";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "모두의 출석 체크를 초기화합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
