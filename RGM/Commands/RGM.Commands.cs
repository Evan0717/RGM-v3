using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MultiBroadcast.API;
using UnityEngine;

namespace RGM.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Report : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string args = string.Join(" ", arguments).Trim();

            if (args == "")
            {
                response = "보낼 메세지를 입력해주세요.";
                return false;
            }
            else
            {
                Discord.Webhook.Send($"## 랜덤게임모드({Server.Port}) \n**{player.Nickname}**({player.Id}, {player.UserId})\n```\n{args}```", 
                    "https://discord.com/api/webhooks/1286570523924627478/oIkgSYPAHul8pKB1tqqXWk3hvVocJBzoWOQTPu0Ha9KmF08NmzXbB3PsY6c7RVg3th6Z");

                response = "서버 관리자에게 메세지가 전달되었습니다.\n유저 정보도 같이 전송되므로, 언행에 주의하십시오.";
                return true;
            }
        }

        public string Command { get; } = "report";

        public string[] Aliases { get; } = { "rep", "ㄱ데", "ㄱ데ㅐㄱㅅ", "문의", "신고", "건의" };

        public string Description { get; } = "[RGM] 관리자에게 매세지를 보낼 수 있습니다.";

        public bool SanitizeResponse { get; } = true;
    }

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

        public string[] Aliases { get; } = { "sl", "scp", "니", "ㄴ체", "ㄴ체ㅣㅑㄴㅅ" };

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
    public class Invisible : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(arguments.At(0));

            player.ChangeAppearance(PlayerRoles.RoleTypeId.None, true);

            response = "Invisible Complete!";

            return true;
        }

        public string Command { get; } = "invisble";

        public string[] Aliases { get; } = { "ivb" };

        public string Description { get; } = "특정 유저를 투명화 상태로 만듭니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
