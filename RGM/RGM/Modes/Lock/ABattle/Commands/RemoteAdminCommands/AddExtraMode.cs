using System;
using CommandSystem;
using Exiled.API.Features;

using RGM.API.Features;

namespace RGM.Modes.Commands;

public class AddExtraMode : ICommand
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
                ABattle.CurrentExtraModes.Add(args);
                string extraMode = $"<size=25><b><color=#fecdcd>{ABattle.CurrentExtraModes}</color></b></size>\n<size=20>{ABattle.ExtraModes[args]}</size>";
                
                foreach (var p in PlayerManager.List)
                {
                    p.AddBroadcast(10, extraMode);
                    p.SendConsoleMessage(extraMode, "");
                }

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