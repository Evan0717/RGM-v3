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
    public class AddBadge : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string userId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            bool flag = userId.AddBadge(args, out response, arguments);
            return flag;
        }

        public string Command { get; } = "addbadge";

        public string[] Aliases { get; } = { "칭호추가" };

        public string Description { get; } = "특정 유저에게 칭호를 지급합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RemoveBadge : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string UserId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            if (arguments.Count < 2)
            {
                response = "플레이어 이름과 칭호 이름을 입력해주세요.\n-";
                return false;
            }
            else
            {
                List<string> uc = UsersManager.UsersCache[UserId];

                if (uc[10] == "0")
                {
                    response = "보유한 칭호가 없습니다.\n-";
                    return false;
                }
                else
                {
                    if (uc[10].Split('/').Contains(args))
                    {
                        List<string> Badges = uc[10].Split('/').ToList();

                        Badges.Remove(args);

                        uc[10] = string.Join("/", Badges);
                        if (uc[10] == "") uc[10] = "0";
                        if (uc[11] == args) uc[11] = "0";
                        UsersManager.UsersCache[UserId] = uc;
                        response = "칭호 제거 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                    else
                    {
                        response = "보유한 칭호가 없습니다.\n-";
                        return false;
                    }
                }
            }
        }

        public string Command { get; } = "removebadge";

        public string[] Aliases { get; } = { "칭호제거" };

        public string Description { get; } = "특정 유저가 보유한 칭호를 제거합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
