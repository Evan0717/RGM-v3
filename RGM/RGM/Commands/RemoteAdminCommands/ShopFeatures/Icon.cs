using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;

using PlayerRoles;
using RGM.API;
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Commands.RemoteAdminCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddIcon : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string userId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            bool flag = userId.AddIcon(args, out response, arguments);
            return flag;
        }

        public string Command { get; } = "addicon";

        public string[] Aliases { get; } = { "아이콘추가" };

        public string Description { get; } = "특정 유저에게 아이콘을 지급합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RemoveIcon : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string UserId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            if (arguments.Count < 2)
            {
                response = "플레이어 이름과 아이콘 이름을 입력해주세요.\n-";
                return false;
            }
            else
            {
                List<string> uc = UsersManager.UsersCache[UserId];

                if (uc[225] == "0")
                {
                    response = "보유한 아이콘이 없습니다.\n-";
                    return false;
                }
                else
                {
                    if (uc[24].Split('/').Contains(args))
                    {
                        List<string> Effects = uc[24].Split('/').ToList();

                        Effects.Remove(args);

                        uc[24] = string.Join("/", Effects);
                        if (uc[24] == "") uc[24] = "0";
                        if (uc[25] == args) uc[25] = "0";
                        UsersManager.UsersCache[UserId] = uc;
                        response = "아이콘 제거 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                    else
                    {
                        response = "보유한 아이콘에 해당 아이콘이 없습니다.\n-";
                        return false;
                    }
                }
            }
        }

        public string Command { get; } = "removeicon";

        public string[] Aliases { get; } = { "아이콘제거" };

        public string Description { get; } = "특정 유저가 보유한 아이콘을 제거합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
