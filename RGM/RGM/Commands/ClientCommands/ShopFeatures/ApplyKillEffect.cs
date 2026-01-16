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
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ApplyKillEffect : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string args = string.Join(" ", arguments).Trim();

            if (args == "")
            {
                response = "킬 이펙트 이름을 입력해주세요.\n-";
                return false;
            }
            else
            {
                if (UsersManager.UsersCache.ContainsKey(player.UserId))
                {
                    List<string> uc = UsersManager.UsersCache[player.UserId];

                    if (args == "0")
                    {
                        uc[4] = "0";
                        UsersManager.UsersCache[player.UserId] = uc;
                        response = "킬 이펙트 장착 해제 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                    if (args == "랜덤 적용")
                    {
                        uc[15] = "1";
                        UsersManager.UsersCache[player.UserId] = uc;
                        response = "킬이펙트가 킬마다 랜덤으로 적용됩니다.";

                        UsersManager.SaveUsers();

                        return true;
                    }
                    if (args == "랜덤 해제")
                    {
                        uc[15] = "0";
                        UsersManager.UsersCache[player.UserId] = uc;
                        response = "이제 킬이펙트가 킬마다 랜덤으로 적용되지 않습니다.";

                        UsersManager.SaveUsers();

                        return true;
                    }
                    else if (KillEffects.ContainsKey(args) && uc[3].Split('/').Contains(args))
                    {
                        uc[4] = args;
                        UsersManager.UsersCache[player.UserId] = uc;
                        response = "킬 이펙트 장착 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                    else
                    {
                        response = "존재하지 않거나 보유하지 않은 킬 이펙트입니다.\n-";
                        return false;
                    }
                }
                else
                {
                    response = "플레이어 정보를 찾을 수 없습니다.\n-";
                    return false;
                }
            }
        }

        public string Command { get; } = "킬이펙트";

        public string[] Aliases { get; } = { "ake" };

        public string Description { get; } = "[RGM] 킬 이펙트 이름을 입력하여 장착을 변경합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
