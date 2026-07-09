using System;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using static RGM.Variables.Variable;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Suicide : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            if (!EnabledModeList.Exists(x => ScpSuicideAvailableModes.Contains(x.Data.Type))
                                             && player.IsScp)
            {
                response = "SCP는 이 명령어를 사용할 수 없습니다.";
                return false;
            }

            if (EnabledModeList.Exists(x => SuicideBlockedModes.Contains(x.Data.Type))
                || SuicideBlockedRoles.Contains(player.Role.Type))
            {
                response = "이 모드에서는 이 명령어를 사용할 수 없습니다.";
                return false;
            }

            if (player.IsAlive && Round.IsStarted)
            {
                player.Kill(DamageType.Poison);

                response =  "당신의 기도는 저 하늘에 닿았습니다.";
                return true;
            }
            
            response =  "이미 하늘나라에 있는 상태입니다.";
            return false;
        }

        public string Command => "suicide";

        public string[] Aliases => ["자살", "살자"];

        public string Description => "[RGM] 스스로 생을 마감할 수 있습니다.";

        public bool SanitizeResponse => true;
    }
}
