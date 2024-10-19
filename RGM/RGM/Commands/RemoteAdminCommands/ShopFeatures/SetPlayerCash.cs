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
            Player player = Player.Get(arguments.At(0));
            int cash = int.Parse(arguments.At(1));

            if (cash < 0)
            {
                response = "0 이상의 값을 입력해주세요.\n-";
                return false;
            }
            else
            {
                List<string> uc = UsersManager.UsersCache[player.UserId];

                uc[2] = cash.ToString();
                UsersManager.UsersCache[player.UserId] = uc;
                response = "캐쉬 설정 완료!\n-";

                UsersManager.SaveUsers();
                return true;
            }
        }

        public string Command { get; } = "setplayercash";

        public string[] Aliases { get; } = { "spc", "캐쉬" };

        public string Description { get; } = "특정 유저의 캐쉬를 정합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
