using AdminToys;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using Mirror;
using PlayerRoles;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using RGM.API;
using RGM.API.Components;
using RGM.API.Features;
using RGM.Modes;
using RGM.Modes.SubClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using UnityEngine;
using static PlayerList;
using static RGM.Variables.Variable;
using static UnityEngine.GraphicsBuffer;

namespace RGM.Commands.RemoteAdminCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Develop : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            bool flag = UsersManager.UsersCache[player.UserId][23] == "1";

            if (flag)
            {
                UsersManager.UsersCache[player.UserId][23] = "0";
                UsersManager.SaveUsers();

                response = "방해 금지 <color=red>OFF</color>!\nComplete!";

                return true;
            }
            else
            {
                UsersManager.UsersCache[player.UserId][23] = "1";
                UsersManager.SaveUsers();

                response = "방해 금지 <color=green>ON</color>!\nComplete!";

                return true;
            }
        }

        public string Command { get; } = "develop";

        public string[] Aliases { get; } = { "dv", "dev", "개발", "roqkf", "dnd", "방해금지" };

        public string Description { get; } = "개발하러 갈 때 사용하세요!";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Test : ICommand, IUsageProvider
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "플레이어 지정 필요";
                return false;
            }
            Player player = Player.Get(arguments.At(0));
            if (player == null)
            {
                response = "플레이어 없음";
                return false;
            }

            Steve.Create(player);
            response = "적용 완료";
            return true;
        }

        public string Command { get; } = "test";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "테스트용";

        public bool SanitizeResponse { get; } = true;

        public string[] Usage { get; } = { "%player%" };
    }
}
