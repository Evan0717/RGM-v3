using CommandSystem;
using DAONTFT.Core.TFT;
using Exiled.API.Features;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using MEC;
using RGM.API.Features;
using System;
using System.Linq;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;

namespace RGM.Modes.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ShowHint : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get(sender);
        var x = arguments.At(0);
        var y = arguments.At(1);
        var text = string.Join(" ", arguments.Skip(2).ToArray());

        Hint hint = new Hint
        {
            Text = text,
            Id = "test",
            XCoordinate = float.Parse(x),
            YCoordinate = float.Parse(y),
            Alignment = HintAlignment.Center,
        };

        player.AddCustomHint(hint);

        Timing.CallDelayed(5, () =>
        {
            player.RemoveHint(hint);
        });

        response = "힌트를 표시했습니다.";
        return true;
    }

    public string Command { get; } = "showhint";

    public string[] Aliases { get; } = {};

    public string Description { get; } = "DAONㅣ힌트를 표시합니다.";
}