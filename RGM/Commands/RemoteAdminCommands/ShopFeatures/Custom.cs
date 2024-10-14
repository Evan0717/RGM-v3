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
using RGM.Features;
using RGM.Modes;
using UnityEngine;

namespace RGM.Commands.RemoteAdminCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddCustom : ICommand
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
            else if (RGM.Instance.Customizations.ContainsKey(args))
            {
                List<string> uc = UsersManager.UsersCache[UserId];

                if (uc[7] == "0")
                {
                    uc[7] = args;
                    UsersManager.UsersCache[UserId] = uc;
                    response = "커스텀 기능 추가 완료!\n-";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    if (uc[7].Contains(args))
                    {
                        response = "이미 해당 커스텀 기능을 보유 중입니다.\n-";
                        return false;
                    }
                    else
                    {
                        uc[7] += $"/{args}";
                        UsersManager.UsersCache[UserId] = uc;
                        response = "커스텀 기능 추가 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                }
            }
            else
            {
                response = "존재하지 않는 커스텀 기능 이름입니다.\n-";
                return false;
            }
        }

        public string Command { get; } = "addcustomfeature";

        public string[] Aliases { get; } = { "acf" };

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
                    if (uc[7].Contains(args))
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

        public string[] Aliases { get; } = { "rcf" };

        public string Description { get; } = "특정 유저가 보유한 커스텀 기능을 제거합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
