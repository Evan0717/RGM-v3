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
    public class SetPlayerPR : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string userId = Tools.TryGetUserId(arguments.At(0));
            int rp = int.Parse(arguments.At(1));

            if (rp < 0)
            {
                response = "0 이상의 값을 입력해주세요.\n-";
                return false;
            }
            else
            {
                List<string> uc = UsersManager.UsersCache[userId];

                uc[1] = rp.ToString();
                UsersManager.UsersCache[userId] = uc;
                response = "RP 설정 완료!\n-";

                UsersManager.SaveUsers();
                return true;
            }
        }

        public string Command { get; } = "setplayerrp";

        public string[] Aliases { get; } = { "spr", "rp" };

        public string Description { get; } = "특정 유저의 RP를 정합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
