using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using ProjectMER.Features;
using ProjectMER.Features.Serializable;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API;
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

namespace RGM.Commands.RemoteAdminCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Develop : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            player.Kill("이 자는 개발의 의무를 짊어지고 죽었습니다.");
            player.Role.Set(RoleTypeId.Overwatch);

            response = "Complete!";

            return true;
        }

        public string Command { get; } = "develop";

        public string[] Aliases { get; } = { "dv", "dev", "개발", "roqkf" };

        public string Description { get; } = "개발하러 갈 때 사용하세요!";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Test : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            float radius = float.Parse(arguments.At(0));
            int pointCount = int.Parse(arguments.At(1));

            List<Vector3> Points = Tools.GetCirclePoints(player.Position, radius, pointCount);

            foreach (var point in Points)
            {
                SerializablePrimitive primitiveSerializable = new SerializablePrimitive
                {
                    PrimitiveType = PrimitiveType.Sphere,
                    Position = point
                };
                ObjectSpawner.SpawnPrimitive(primitiveSerializable);
            }    

            response = "Complete!";

            return true;
        }

        public string Command { get; } = "test";

        public string[] Aliases { get; } = {};

        public string Description { get; } = "테스트용 명령어";

        public bool SanitizeResponse { get; } = true;
    }
}
