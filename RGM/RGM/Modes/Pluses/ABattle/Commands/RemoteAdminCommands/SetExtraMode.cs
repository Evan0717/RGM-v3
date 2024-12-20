using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using MultiBroadcast.API;

namespace RGM.Modes.Commands;

public class SetExtraMode : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (Round.IsStarted)
        {
            var args = string.Join(" ", arguments);
            var player = Player.Get(sender);

            if (!ABattle.ExtraModes.ContainsKey(args))
            {
                response = $"올바른 추가 모드를 입력해주세요.\n{string.Join(", ", ABattle.ExtraModes.Keys)}\nSetExtraMode";
                return false;
            }
            else
            {
                ABattle.Instance.CurrentExtraMode = args;
                string extraMode = $"<size=25><b><color=#fecdcd>{ABattle.Instance.CurrentExtraMode}</color></b></size>\n<size=20>{ABattle.ExtraModes[ABattle.Instance.CurrentExtraMode]}</size>";
                player.AddBroadcast(10, extraMode);
                player.SendConsoleMessage(extraMode, "");

                response = "SetExtraMode Complete!";
                return true;
            }
        }

        response = "라운드 시작 전에는 사용할 수 없습니다.";

        return false;
    }

    public string Command { get; } = "setextramode";

    public string[] Aliases { get; } = { "sem", "엑스트라", "엑스트라모드", "추가모드" };

    public string Description { get; } = "워크스테이션 업그레이드ㅣ추가 모드를 정합니다.";
}