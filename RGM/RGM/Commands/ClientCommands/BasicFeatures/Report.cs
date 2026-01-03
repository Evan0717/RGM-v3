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
using RGM.API.Components;
using RGM.Modes;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Report : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string args = string.Join(" ", arguments).Trim();

            if (args == "")
            {
                response = "보낼 메세지를 입력해주세요.";
                return false;
            }
            else
            {
                DiscordInteraction.Discord.Webhook.Send($"{player.Nickname}({player.Id}, {player.UserId}) {args}",
                    "https://discord.com/api/webhooks/1286570523924627478/oIkgSYPAHul8pKB1tqqXWk3hvVocJBzoWOQTPu0Ha9KmF08NmzXbB3PsY6c7RVg3th6Z");

                response = "서버 관리자에게 메세지가 전달되었습니다.\n유저 정보도 같이 전송되므로, 언행에 주의하십시오.";
                return true;
            }
        }

        public string Command { get; } = "report";

        public string[] Aliases { get; } = { "문의" };

        public string Description { get; } = "[RGM] 관리자에게 매세지를 보낼 수 있습니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
