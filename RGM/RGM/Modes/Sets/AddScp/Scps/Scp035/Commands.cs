using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;

namespace RGM.Modes.Sets.AddScp.Scps;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SetScp035 : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get(arguments.At(0));

        Scp035.Create(player);

        response = $"{player.DisplayNickname}을(를) SCP-035로 만드는데 성공했습니다.";
        return true;
    }

    public string Command { get; } = "setscp035";

    public string[] Aliases { get; } = { "scp035" };

    public string Description { get; } = "추가 SCPㅣ특정 유저를 SCP-035로 만듭니다.";
}