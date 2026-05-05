using System;
using CommandSystem;
using Exiled.API.Features;

namespace RGM.Commands.RemoteAdminCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class PrintPos : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string info = $"new Vector3({player.Position.x}, {player.Position.y}, {player.Position.z})";
            Log.Info(info);

            response = $"현재 위치: {info}";
            return true;
        }

        public string Command { get; } = "printpos";

        public string[] Aliases { get; } = { "print" };

        public string Description { get; } = "현재의 위치 정보를 콘솔에 출력합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
