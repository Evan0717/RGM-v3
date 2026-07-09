using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using RGM.API.Features;

namespace RGM.RGM.Commands.RemoteAdminCommands.AdminFeatures;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class RandomItem : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.GivingItems))
        {
            response = "이 명령어에 대한 권한이 없습니다.";
            return false;
        }

        var translator = sender is not null
                         && !sender.CheckPermission(PlayerPermissions.ServerConsoleCommands)
                         && Player.Get(sender).IsUsingTranslator();
        switch (arguments.Count)
        {
            case <= 0:
                response = translator
                    ? """
                      Usage: 
                      /randomItem <count> <username (optional)>
                      """
                    : """
                      사용 방법: 
                      /randomItem <갯수> <사용자 이름(추가항목)>
                      """;
                return false;
            case 1:
                try
                {
                    var playerList
                        = PlayerManager.List.Where(x => !x.IsNPC && !x.IsNonePlayer() && !x.IsDead);
                    IEnumerable<Player> list = playerList.ToList();
                    if (list.ToList().Count == 0)
                    {
                        response = translator
                            ? "No available player found. Please specify a player name."
                            : "가능한 플레이어를 찾을 수 없습니다. 플레이어 이름을 지정해보세요.";
                        return false;
                    }

                    var player = list.ToList().GetRandomValue();
                    if (!int.TryParse(arguments.At(0), out var count))
                    {
                        response = translator
                            ? "The number is not appropriate."
                            : "숫자가 알맞지 않습니다.";
                        return false;
                    }

                    for (var i = 0; i < count; i++)
                        GiveItem(player);

                    response = translator
                        ? $"Player {player.Nickname} has been given {count} items."
                        : $"플레이어 {player.Nickname}에게 {count}개의 아이템을(를) 지급하였습니다.";
                    return true;
                }
                catch (Exception e)
                {
                    response = translator
                        ? """
                          An unknown error has occurred.
                          The error has been reported to the server; if the issue persists, please contact the administrator.
                          """
                        : """
                          알수 없는 오류가 발생하였습니다.
                          해당 오류는 서버에 제출되며, 계속 반복 시 담당 관리자에게 문의해주세요.
                          """;
                    Log.Error(e);
                    return false;
                }
        }

        try
        {
            if (!Player.TryGet(arguments.At(1), out var result))
            {
                response = $"플레이어 {arguments.At(1)}을(를) 찾을 수 없습니다.";
                return false;
            }

            if (result.IsNPC || result.IsNonePlayer() || result.IsDead)
            {
                response = translator
                    ? $"Player {arguments.At(1)}'s data is damaged or does not exist."
                    : $"""
                       플레이어 {arguments.At(1)}의 데이터가 손상되었거나 존재하지 않습니다.
                       만약 플레이어가 더미라면 사용할 수 없는 것입니다. :(
                       """;
                return false;
            }

            response = translator
                ? $"Player {result.Nickname} has been given {GiveItem(result)} items."
                : $"플레이어 {result.Nickname}에게 {GiveItem(result)}을(를) 지급하였습니다.";
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e);
            response = translator
                ? """
                  An unknown error has occurred.
                  The error has been reported to the server; if the issue persists, please contact the administrator.
                  """
                : """
                  알수 없는 오류가 발생하였습니다.
                  해당 오류는 서버에 제출되며, 계속 반복 시 담당 관리자에게 문의해주세요.
                  """;
            return false;
        }

        // -----------------
        Item GiveItem(Player player)
            => player.AddRandomItem();
    }

    public string Command => "AddRandomItem";
    public string[] Aliases => ["randomItem", "랜덤뽑기"];
    public string Description => "무작위 아이템을 추가합니다.";
    public string[] Usage => ["Count", "PlayerName(Option)"];
}