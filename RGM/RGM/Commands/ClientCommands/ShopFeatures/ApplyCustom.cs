using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;

using PlayerRoles;
using RGM.API;
using RGM.API.Components;
using RGM.Modes;
using UnityEngine;
using RGM.API.Features;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ApplyChangeDisplayNickname : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string args = string.Join(" ", arguments).Trim();

            if (UsersManager.UsersCache.ContainsKey(player.UserId))
            {
                List<string> uc = UsersManager.UsersCache[player.UserId];

                if (uc[7].Split('/').Contains("커스텀 닉네임"))
                {
                    uc[5] = args == "" ? "0" : args;
                    UsersManager.UsersCache[player.UserId] = uc;

                    response = "닉네임 변경 완료!\n-";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    response = "해당 기능은 상점에서 구매할 수 있습니다.\n-";
                    return false;
                }
            }
            else
            {
                response = "플레이어 정보를 찾을 수 없습니다.\n-";
                return false;
            }
        }

        public string Command { get; } = "applychangedisplaynickname";

        public string[] Aliases { get; } = { "acdn", "닉네임" };

        public string Description { get; } = "[RGM] 다른 유저에게 보여지는 이름을 수정합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ApplyChangeCustomInfo : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string args = string.Join(" ", arguments).Trim();

            if (UsersManager.UsersCache.ContainsKey(player.UserId))
            {
                List<string> uc = UsersManager.UsersCache[player.UserId];

                if (uc[7].Split('/').Contains("커스텀 인포"))
                {
                    uc[6] = args == "" ? "0" : args;
                    UsersManager.UsersCache[player.UserId] = uc;

                    response = "인포 변경 완료!\n-";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    response = "해당 기능은 상점에서 구매할 수 있습니다.\n-";
                    return false;
                }
            }
            else
            {
                response = "플레이어 정보를 찾을 수 없습니다.\n-";
                return false;
            }
        }

        public string Command { get; } = "인포";

        public string[] Aliases { get; } = { "acci" };

        public string Description { get; } = "[RGM] 다른 유저에게 보여지는 역할 설명을 수정합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
