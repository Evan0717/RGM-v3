using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;

namespace RGM.Modes.Sets.AddScp.Scps;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SetScp1162 : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Scp1162.OnEnabled();

        response = $"SCP-1162를 추가하는데 성공했습니다.";
        return true;
    }

    public string Command { get; } = "setscp1162";

    public string[] Aliases { get; } = { "scp1162" };

    public string Description { get; } = "추가 SCPㅣ시설에 SCP-1162를 추가합니다.";
}