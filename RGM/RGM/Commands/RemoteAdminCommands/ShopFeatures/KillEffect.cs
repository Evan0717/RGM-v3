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
    public class AddKillEffect : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string userId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            bool flag = userId.AddKillEffect(args, out response, arguments);
            return flag;
        }

        public string Command { get; } = "addkilleffect";

        public string[] Aliases { get; } = { "킬이펙트추가" };

        public string Description { get; } = "특정 유저에게 킬이펙트를 지급합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RemoveKillEffect : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string UserId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            if (arguments.Count < 2)
            {
                response = "플레이어 이름과 킬이펙트 이름을 입력해주세요.\n-";
                return false;
            }
            else
            {
                List<string> uc = UsersManager.UsersCache[UserId];

                if (uc[3] == "0")
                {
                    response = "보유한 킬이펙트가 없습니다.\n-";
                    return false;
                }
                else
                {
                    if (uc[3].Split('/').Contains(args))
                    {
                        List<string> Effects = uc[3].Split('/').ToList();

                        Effects.Remove(args);

                        uc[3] = string.Join("/", Effects);
                        if (uc[3] == "") uc[3] = "0";
                        if (uc[4] == args) uc[4] = "0";
                        UsersManager.UsersCache[UserId] = uc;
                        response = "킬이펙트 제거 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                    else
                    {
                        response = "보유한 킬이펙트에 해당 킬이펙트가 없습니다.\n-";
                        return false;
                    }
                }
            }
        }

        public string Command { get; } = "removekilleffect";

        public string[] Aliases { get; } = { "킬이펙트제거" };

        public string Description { get; } = "특정 유저가 보유한 킬이펙트를 제거합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
