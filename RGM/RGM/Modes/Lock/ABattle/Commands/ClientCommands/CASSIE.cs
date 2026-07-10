using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Commands;

public class CASSIE : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        var player = Player.Get(sender);
        var args = string.Join(" ", arguments);

        if (player.Role.Type == RoleTypeId.Scp079 && ABattleVar.CASSIE.ContainsKey(player) && ABattleVar.CASSIE[player] > 0)
        {
            ABattleVar.CASSIE[player]--;

            Tools.MessageTranslated("", $"<color=red>SCP-079</color>(<b><i>{player.DisplayNickname}</i></b>): {args}");

            response = $"\n전송 완료!\nC.A.S.S.I.E.";
            return true;
        }
        else
        {
            response = "\nCASSIE 능력 사용 조건에 맞지 않습니다.\nSCP-079가 아니거나 사용 횟수를 모두 소모했습니다.";
            return false;
        }
    }

    public string Command { get; } = "cassie";
    public string[] Aliases { get; } = { "캐시" };
    public string Description { get; } = "워크스테이션 업그레이드ㅣSCP-079 전용 능력입니다. 모두에게 말을 전달할 수 있습니다.";
}