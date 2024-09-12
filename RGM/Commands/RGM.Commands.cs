using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using MultiBroadcast.API;
using UnityEngine;

namespace RGM.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ScpList : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player.IsScp)
            {
                response = "\n" + string.Join("\n", Player.List.Where(x => x.IsScp).Select(x => $"{x.Role.Name} : {x.Nickname}"));
                return true;
            }
            else
            {
                response = "SCP만 사용할 수 있는 명령어입니다.";
                return false;
            }
        }

        public string Command { get; } = "scplist";

        public string[] Aliases { get; } = { "sl", "scp" };

        public string Description { get; } = "[RGM] 존재하는 SCP 리스트를 나열합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class CurrentMode : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string CurrentMode = RGM.Instance.CurrentMode;
            if (RGM.Instance.CurrentMode == null)
            {
                response = "현재 모드가 설정되지 않았습니다.";
                return false;
            }
            else
            {
                string ModeDescription = RGM.Instance.ModeList[CurrentMode][1];

                response = $"\n[ {CurrentMode} ]\n" +
                    $"------------------------------------------------------------------------" +
                    $"\n{ModeDescription}\n" +
                    $"------------------------------------------------------------------------";

                return true;
            }
        }

        public string Command { get; } = "currentmode";

        public string[] Aliases { get; } = { "모드", "mode" };

        public string Description { get; } = "[RGM] 현재 모드를 확인합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Chat : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (RGM.Instance.ChatCooldown.Contains(player))
            {
                response = "너무 빠른 간격으로 입력을 보내고 있습니다!";
                return false;
            }
            else
            {
                RGM.Instance.ChatCooldown.Add(player);

                if (player.IsScp)
                {
                    if (arguments.Count == 0)
                    {
                        response = "보낼 메세지를 입력해주세요.";
                        return false;
                    }

                    string text = player.Role.Name;
                    string text2 = string.Concat(new string[]
                    {
                        $"<size=25><color={player.Role.Color.ToHex()}>",
                        text,
                        $"</color> ({player.Nickname}) <b> | </b>",
                        string.Join(" ", arguments),
                        "</size>"
                    });
                    foreach (Player ply in Player.List.Where(x => x.IsScp))
                    {
                        ply.AddBroadcast(6, text2);
                    }
                    response = $"'{text2}'";
                    return true;
                }
                else if (player.IsDead)
                {
                    string text = player.Role.Name;
                    string text2 = string.Concat(new string[]
                    {
                        $"<size=25><color={player.Role.Color.ToHex()}>",
                        text,
                        $"</color> ({player.Nickname}) <b> | </b>",
                        string.Join(" ", arguments),
                        "</size>"
                    });

                    foreach (Player ply in Player.List.Where(x => x.IsDead))
                        ply.AddBroadcast(6, text2);

                    response = $"'{text2}'";
                    return true;
                }
                else
                {
                    string text = player.Role.Name;
                    string text2 = string.Concat(new string[]
                    {
                        $"<size=25><color={player.Role.Color.ToHex()}>",
                        text,
                        $"</color> ({player.Nickname}) <b> | </b>",
                        string.Join(" ", arguments),
                        "</size>"
                    });

                    foreach (Player ply in Player.List)
                    {
                        if (Vector3.Distance(ply.Position, player.Position) <= 10 || ply.IsDead)
                            ply.AddBroadcast(6, text2);
                    }
                    
                    response = $"'{text2}'";
                    return true;
                }
            }
        }

        public string Command { get; } = "chat";
        public string[] Aliases { get; } = new string[] { "챗", "채팅", "ㅊ", "c" };
        public string Description { get; } = "[RGM] 텍스트 채팅을 사용할 수 있습니다.";
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceMode : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            string args = string.Join(" ", arguments).Trim();

            if (args == "")
            {
                List<string> ModeList = new List<string>();

                foreach (var Mode in RGM.Instance.ModeList.Keys)
                    ModeList.Add($"{Mode}");

                response = $"<b><size=30>[ 모드 리스트 ]</b></size>\n{string.Join(", ", ModeList)}\nSending Command Error..";
                return false;
            }
            else if (RGM.Instance.ModeList.Keys.Contains(args))
            {
                RGM.Instance.CurrentMode = args;
                Server.ExecuteCommand($"/cassie_sl <mark=#ffff00aa><color=#000000>관리자(<color=#ffffff>{player.Nickname}</color>)에 의하여 이번 라운드의 모드가 <b>{args}</b>으로 확정되었습니다.</color></mark>");
                response = $"이번 라운드의 모드는 <b>{args}</b>입니다.\nSending Command Complete!";
                return true;
            }
            else
            {
                response = "존재하지 않는 <모드 이름>입니다.\nSending Command Error..";
                return false;
            }

        }

        public string Command { get; } = "forcemode";

        public string[] Aliases { get; } = { "fm" };

        public string Description { get; } = "'/fm <모드 이름>'ㅣ현재 라운드의 모드를 강제합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceModeReset : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            RGM.Instance.PickModes();
            response = "모드 리셋이 완료되었습니다.";
            return true;
        }

        public string Command { get; } = "forcemodereset";

        public string[] Aliases { get; } = { "fmr" };

        public string Description { get; } = "'/fmrㅣ모드 투표를 강제로 리셋합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Test : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "테스트 성공입니다. 제대로 작동하네요.\nHello, World!";

            return true;
        }

        public string Command { get; } = "test";

        public string[] Aliases { get; } = null;

        public string Description { get; } = "테스트용 명령어";

        public bool SanitizeResponse { get; } = true;
    }
}
