using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;

namespace RGM.Modes.Sets.AddScp.Scps;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class AddScp005 : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get(arguments.At(0));

        Item item = Scp005.Create();
        player.AddItem(item);

        response = $"{player.DisplayNickname}에(게) SCP-005를 지급했습니다.";
        return true;
    }

    public string Command { get; } = "addscp005";

    public string[] Aliases { get; } = { "scp005" };

    public string Description { get; } = "추가 SCPㅣ특정 유저에게 SCP-005를 지급합니다.";
}