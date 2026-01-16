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
    public class AddSpawnEffect : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string UserId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            if (arguments.Count < 2)
            {
                response = "스폰이펙트추가 <player> <spawn effect name>";
                return false;
            }
            else if (SpawnEffects.ContainsKey(args))
            {
                List<string> uc = UsersManager.UsersCache[UserId];

                if (uc[19] == "0")
                {
                    uc[19] = args;
                    UsersManager.UsersCache[UserId] = uc;
                    response = "Successfully add spawn effect.";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    if (uc[19].Split('/').Contains(args))
                    {
                        response = "This player already have this spawn effect.";
                        return false;
                    }
                    else
                    {
                        uc[19] += $"/{args}";
                        UsersManager.UsersCache[UserId] = uc;
                        response = "Successfully add spawn effect.";

                        UsersManager.SaveUsers();
                        return true;
                    }
                }
            }
            else
            {
                response = "This spawn effect is not exist.";
                return false;
            }
        }

        public string Command { get; } = "addspawneffect";

        public string[] Aliases { get; } = { "스폰이펙트추가" };

        public string Description { get; } = "특정 유저에게 스폰 이펙트를 지급합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RemoveSpawnEffect : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string UserId = Tools.TryGetUserId(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            if (arguments.Count < 2)
            {
                response = "플레이어 이름과 스폰 이펙트 이름을 입력해주세요.\n-";
                return false;
            }
            else
            {
                List<string> uc = UsersManager.UsersCache[UserId];

                if (uc[19] == "0")
                {
                    response = "보유한 스폰 이펙트가 없습니다.\n-";
                    return false;
                }
                else
                {
                    if (uc[19].Split('/').Contains(args))
                    {
                        List<string> Effects = uc[19].Split('/').ToList();

                        Effects.Remove(args);

                        uc[19] = string.Join("/", Effects);
                        if (uc[19] == "") uc[19] = "0";
                        if (uc[20] == args) uc[20] = "0";
                        UsersManager.UsersCache[UserId] = uc;
                        response = "스폰 이펙트 제거 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                    else
                    {
                        response = "보유한 스폰 이펙트에 해당 스폰 이펙트가 없습니다.\n-";
                        return false;
                    }
                }
            }
        }

        public string Command { get; } = "removespawneffect";

        public string[] Aliases { get; } = { "스폰이펙트제거" };

        public string Description { get; } = "특정 유저가 보유한 스폰 이펙트를 제거합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
