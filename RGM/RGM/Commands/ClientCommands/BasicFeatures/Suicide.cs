using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API;
using RGM.API.Components;
using RGM.Modes;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Suicide : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player.IsAlive && Round.IsStarted)
            {
                player.Kill(DamageType.Warhead);

                response =  "당신의 기도는 저 하늘에 닿았습니다.";
                return true;
            }
            else
            {
                response =  "이미 하늘나라에 있는 상태입니다.";
                return false;
            }
        }

        public string Command { get; } = "suicide";

        public string[] Aliases { get; } = { "자살", "살자" };

        public string Description { get; } =  "[RGM] 스스로 생을 마감할 수 있습니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
