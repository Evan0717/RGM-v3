using CommandSystem;
using Exiled.API.Features;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RGM.Modes.Commands;

public class AbilityInformation : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (Round.IsStarted)
        {
            var args = string.Join(" ", arguments);

            var ability = ABattle.Instance.FindAbility(args);

            if (ability == AbilityType.NONE)
            {
                response = "해당 능력을 찾을 수 없습니다.";
                return false;
            }

            response = ABattle.Instance.GetAbilityInformation(ability);
            return true;
        }

        response = "라운드 시작 전에는 사용할 수 없습니다.";

        return false;
    }

    public string Command { get; } = "능력정보";
    public string[] Aliases { get; } = {"AbilityInformation", "AI"};
    public string Description { get; } = "워크스테이션 업그레이드ㅣ능력의 정보를 확인합니다.";
}