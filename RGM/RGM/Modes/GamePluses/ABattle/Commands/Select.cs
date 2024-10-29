using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Exiled.API.Features;

namespace RGM.Modes.Commands;

public class SelectFirst : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        var player = Player.Get(sender);

        if (player == null)
        {
            response = "플레이어를 찾을 수 없습니다.";
            return false;
        }

        return ABattle.Instance.Select(player, 1, out response);
    }

    public string Command { get; } = "1";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "워크스테이션 업그레이드ㅣ1번 능력 선택";
}

public class SelectSecond : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        var player = Player.Get(sender);

        if (player == null)
        {
            response = "플레이어를 찾을 수 없습니다.";
            return false;
        }

        return ABattle.Instance.Select(player, 2, out response);
    }

    public string Command { get; } = "2";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "워크스테이션 업그레이드ㅣ2번 능력 선택";
}

public class SelectThird : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        var player = Player.Get(sender);

        if (player == null)
        {
            response = "플레이어를 찾을 수 없습니다.";
            return false;
        }

        return ABattle.Instance.Select(player, 3, out response);
    }

    public string Command { get; } = "3";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "워크스테이션 업그레이드ㅣ3번 능력 선택";
}