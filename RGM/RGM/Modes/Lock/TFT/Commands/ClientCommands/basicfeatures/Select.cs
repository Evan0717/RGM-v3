using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using DAONTFT.Core.TFT;
using Exiled.API.Features;

namespace DAONTFT.Core.Commands.ClientCommands.basicfeatures;

public class SelectTFTFirst : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);

        if (player == null)
        {
            response = "플레이어를 찾을 수 없습니다.";
            return false;
        }

        try
        {
            var result = TFTBattle.Select(player, 1, out response);

            return result;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            response = "1번에 할당된 능력이 존재하지 않습니다.";
            return false;
        }
    }

    public string Command { get; } = "1";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "DAONㅣ1번 능력 선택";
}

public class SelectTFTSecond : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);

        if (player == null)
        {
            response = "플레이어를 찾을 수 없습니다.";
            return false;
        }

        try
        {
            var result = TFTBattle.Select(player, 2, out response);

            return result;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            response = "2번에 할당된 능력이 존재하지 않습니다.";
            return false;
        }
    }

    public string Command { get; } = "2";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "DAONㅣ2번 능력 선택";
}