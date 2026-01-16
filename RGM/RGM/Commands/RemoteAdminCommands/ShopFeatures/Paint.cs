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
    public class AddPaint : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string userId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            bool flag = userId.AddPaint(args, out response, arguments);
            return flag;
        }

        public string Command { get; } = "addpaint";

        public string[] Aliases { get; } = { "페인트추가" };

        public string Description { get; } = "특정 유저에게 페인트를 지급합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RemovePaint : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string UserId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            if (arguments.Count < 2)
            {
                response = "플레이어 이름과 페인트 이름을 입력해주세요.\n-";
                return false;
            }
            else
            {
                List<string> uc = UsersManager.UsersCache[UserId];

                if (uc[8] == "0")
                {
                    response = "보유한 페인트가 없습니다.\n-";
                    return false;
                }
                else
                {
                    if (uc[8].Contains(args))
                    {
                        List<string> Paints = uc[8].Split('/').ToList();

                        Paints.Remove(args);

                        uc[8] = string.Join("/", Paints);
                        if (uc[8] == "") uc[8] = "0";
                        if (uc[9] == args) uc[9] = "0";
                        UsersManager.UsersCache[UserId] = uc;
                        response = "페인트 제거 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                    else
                    {
                        response = "보유한 페인트가 없습니다.\n-";
                        return false;
                    }
                }
            }
        }

        public string Command { get; } = "removepaint";

        public string[] Aliases { get; } = { "페인트제거" };

        public string Description { get; } = "특정 유저가 보유한 페인트를 제거합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
