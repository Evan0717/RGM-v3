using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using RGM.API.Features;

namespace RGM.Modes.Commands;

public class AddAbility : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (Round.IsStarted)
        {
            var players = arguments.At(0) == "all" ? PlayerManager.List : new List<Player> { Player.Get(arguments.At(0)) };
            var args = string.Join(" ", arguments.Skip(1));

            if (arguments.Count < 2)
            {
                foreach (var player in players)
                    ABattle.Instance.AddAbility(player, ABattle.Instance.GetRandomAbilities(AbilityCategory.Dummy, 1)[0]);
            }
            else
            {
                var ability = ABattle.Instance.FindAbility(args);

                if (ability == AbilityType.NONE)
                {
                    response = "해당 능력을 찾을 수 없습니다.";
                    return false;
                }

                foreach (var player in players)
                    ABattle.Instance.AddAbility(player, ability);
            }

            response = "AddAbility Complete!";
            return true;
        }

        response = "라운드 시작 전에는 사용할 수 없습니다.";

        return false;
    }

    public string Command { get; } = "addability";

    public string[] Aliases { get; } = { "aa", "add" };

    public string Description { get; } = "워크스테이션 업그레이드ㅣ능력을 추가합니다.";
}