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
    public class AddCustomFeature : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string userId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            bool flag = userId.AddCustom(args, out response, arguments);
            return flag;
        }

        public string Command { get; } = "addcustomfeature";

        public string[] Aliases { get; } = { "커스텀추가" };

        public string Description { get; } = "특정 유저에게 커스텀 기능을 지급합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RemoveCustom : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string UserId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            if (arguments.Count < 2)
            {
                response = "플레이어 이름과 커스텀 기능을 입력해주세요.\n-";
                return false;
            }
            else
            {
                List<string> uc = UsersManager.UsersCache[UserId];

                if (uc[7] == "0")
                {
                    response = "보유한 커스텀 기능이 없습니다.\n-";
                    return false;
                }
                else
                {
                    if (uc[7].Split('/').Contains(args))
                    {
                        List<string> Customs = uc[7].Split('/').ToList();

                        Customs.Remove(args);

                        uc[7] = string.Join("/", Customs);
                        if (uc[7] == "") uc[7] = "0";
                        UsersManager.UsersCache[UserId] = uc;
                        response = "커스텀 기능 제거 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                    else
                    {
                        response = "보유한 커스텀에 해당 기능이 없습니다.\n-";
                        return false;
                    }
                }
            }
        }

        public string Command { get; } = "removecustomfeature";

        public string[] Aliases { get; } = { "커스텀제거" };

        public string Description { get; } = "특정 유저가 보유한 커스텀 기능을 제거합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
