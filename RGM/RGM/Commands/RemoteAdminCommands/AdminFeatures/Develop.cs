using AdminToys;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;

using PlayerRoles;
using ProjectMER.Features;
using ProjectMER.Features.Serializable;
using RGM.API;
using RGM.API.Components;
using RGM.API.Features;
using RGM.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;
using static PlayerList;
using static UnityEngine.GraphicsBuffer;
using static RGM.Variables.Variable;

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
    public class Test : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 100))
            {
                LabApi.Features.Wrappers.PrimitiveObjectToy toy = LabApi.Features.Wrappers.PrimitiveObjectToy.Create();
                toy.Type = PrimitiveType.Sphere;
                toy.Position = hit.point;
                toy.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                toy.Color = Color.red;
                toy.GameObject.AddComponent<BallComponent>();
                toy.Spawn();

                response = "Complete!";
                return true;
            }
            else
            {
                response = "Raycast failed!";
                return false;
            }
        }

        public string Command { get; } = "test";

        public string[] Aliases { get; } = {};

        public string Description { get; } = "테스트용 명령어";

        public bool SanitizeResponse { get; } = true;
    }
}
