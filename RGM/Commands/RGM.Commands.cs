using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API;
using RGM.Features;
using RGM.Modes;
using UnityEngine;

namespace RGM.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Suicide : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player.IsAlive && Round.IsStarted)
            {
                player.Kill("벌레를 피하다가 사망하였습니다.");
                response = "당신의 기도는 저 하늘에 닿았습니다.";
                return true;
            }
            else
            {
                response = "이미 하늘나라에 있는 상태입니다.";
                return false;
            }
        }

        public string Command { get; } = "suicide";

        public string[] Aliases { get; } = { "자살", "wktkf" };

        public string Description { get; } = "[RGM] 스스로 생을 마감할 수 있습니다.";

        public bool SanitizeResponse { get; } = true;
    }

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
                Discord.Webhook.Send($"{player.Nickname}({player.Id}, {player.UserId}) {args}",
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
            Player player = Player.Get(sender);

            string CurrentMode = RGM.Instance.CurrentMode;

            if (arguments.Count == 0)
            {
                if (RGM.Instance.CurrentMode == null)
                {
                    response = "현재 모드가 설정되지 않았습니다.";
                    return false;
                }
                else
                {
                    string ModeColor = RGM.Instance.ModeList[CurrentMode][0];
                    string ModeDescription = RGM.Instance.ModeList[CurrentMode][1];
                    string ModeFileName = RGM.Instance.ModeList[CurrentMode][2];
                    string ModeDescriptionDetail = RGM.Instance.ModeList[CurrentMode][5];

                    string Message = Notions.StartModeDescription
                        .Replace("{ModeColor}", ModeColor)
                        .Replace("{CurrentMode}", CurrentMode)
                        .Replace("{ModeDescription}", ModeDescription);

                    response = $"성공적으로 모드 설명을 불러왔습니다.";

                    player.SendConsoleMessage($"\n{Message}", "white");
                    if (ModeDescriptionDetail == "")
                        player.SendConsoleMessage($"\n해당 모드에 대한 자세한 설명이 없습니다.", "white");

                    else
                        player.SendConsoleMessage($"\n{ModeDescriptionDetail}", "white");

                    return true;
                }
            }
            else
            {
                string args = string.Join(" ", arguments).Trim();

                if (RGM.Instance.ModeList.Keys.Contains(args))
                {
                    CurrentMode = args;

                    string ModeColor = RGM.Instance.ModeList[CurrentMode][0];
                    string ModeDescription = RGM.Instance.ModeList[CurrentMode][1];
                    string ModeFileName = RGM.Instance.ModeList[CurrentMode][2];
                    string ModeDescriptionDetail = RGM.Instance.ModeList[CurrentMode][5];

                    string Message = Notions.StartModeDescription
                        .Replace("{ModeColor}", ModeColor)
                        .Replace("{CurrentMode}", CurrentMode)
                        .Replace("{ModeDescription}", ModeDescription);

                    response = $"성공적으로 모드 설명을 불러왔습니다.";

                    player.SendConsoleMessage($"\n{Message}", "white");
                    if (ModeDescriptionDetail == "")
                        player.SendConsoleMessage($"\n해당 모드에 대한 자세한 설명이 없습니다.", "white");

                    else
                        player.SendConsoleMessage($"\n{ModeDescriptionDetail}", "white");

                    return true;
                }
                else
                {
                    response = "존재하지 않는 <모드 이름>입니다.\nSending Command Error..";
                    return false;
                }
            }
        }

        public string Command { get; } = "currentmode";

        public string[] Aliases { get; } = { "모드", "mode", "mod" };

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

    [CommandHandler(typeof(ClientCommandHandler))]
    public class VoteFirst : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Round.IsStarted)
            {
                Player player = Player.Get(sender);

                RGM.Instance.Requests.Add($"ABattle/{player.Id}/Vote/1");

                response = "1번 능력 선택 완료!";

                return true;
            }
            else
            {
                response = "라운드 시작 전에는 사용할 수 없습니다.";

                return false;
            }
        }

        public string Command { get; } = "1";

        public string[] Aliases { get; } = {};

        public string Description { get; } = "워크스테이션 업그레이드ㅣ1번 능력 선택";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class VoteSecond : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Round.IsStarted)
            {
                Player player = Player.Get(sender);

                RGM.Instance.Requests.Add($"ABattle/{player.Id}/Vote/2");

                response = "2번 능력 선택 완료!";

                return true;
            }
            else
            {
                response = "라운드 시작 전에는 사용할 수 없습니다.";

                return false;
            }
        }

        public string Command { get; } = "2";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "워크스테이션 업그레이드ㅣ2번 능력 선택";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class VoteThird : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Round.IsStarted)
            {
                Player player = Player.Get(sender);

                RGM.Instance.Requests.Add($"ABattle/{player.Id}/Vote/3");

                response = "3번 능력 선택 완료!";

                return true;
            }
            else
            {
                response = "라운드 시작 전에는 사용할 수 없습니다.";

                return false;
            }
        }

        public string Command { get; } = "3";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "워크스테이션 업그레이드ㅣ3번 능력 선택";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class PlayerInfo : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            response = $"성공적으로 플레이어 정보를 불러왔습니다.";

            player.SendConsoleMessage(Tools.GetPlayerInfo(player), "white");

            return true;
        }

        public string Command { get; } = "playerinfo";

        public string[] Aliases { get; } = { "stat", "스텟", "정보", "info" };

        public string Description { get; } = "[RGM] 현재 자신의 정보를 확인합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ApplyKillEffect : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string args = string.Join(" ", arguments).Trim();

            if (args == "")
            {
                response = "킬 이펙트 이름을 입력해주세요.\n-";
                return false;
            }
            else
            {
                if (UsersManager.UsersCache.ContainsKey(player.UserId))
                {
                    List<string> uc = UsersManager.UsersCache[player.UserId];

                    if (args == "0")
                    {
                        uc[4] = "0";
                        UsersManager.UsersCache[player.UserId] = uc;
                        response = "킬 이펙트 장착 해제 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                    else if (RGM.Instance.KillEffects.ContainsKey(args) && uc[3].Split('/').Contains(args))
                    {
                        uc[4] = args;
                        UsersManager.UsersCache[player.UserId] = uc;
                        response = "킬 이펙트 장착 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                    else
                    {
                        response = "존재하지 않거나 보유하지 않은 킬 이펙트입니다.\n-";
                        return false;
                    }
                }
                else
                {
                    response = "플레이어 정보를 찾을 수 없습니다.\n-";
                    return false;
                }
            }
        }

        public string Command { get; } = "applykilleffect";

        public string[] Aliases { get; } = { "킬이펙트장착", "킬이펙트" };

        public string Description { get; } = "[RGM] 킬 이펙트 이름을 입력하여 장착을 변경합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ApplyChangeDisplayNickname : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string args = string.Join(" ", arguments).Trim();

            if (UsersManager.UsersCache.ContainsKey(player.UserId))
            {
                List<string> uc = UsersManager.UsersCache[player.UserId];

                if (int.Parse(uc[2]) >= 50000)
                {
                    uc[5] = args == "" ? "0" : args;
                    UsersManager.UsersCache[player.UserId] = uc;
                    player.DisplayNickname = args;

                    response = "닉네임 변경 완료!\n-";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    response = "해당 기능은 50,000원 후원 혜택입니다.\n-";
                    return false;
                }
            }
            else
            {
                response = "플레이어 정보를 찾을 수 없습니다.\n-";
                return false;
            }
        }

        public string Command { get; } = "applychangedisplaynickname";

        public string[] Aliases { get; } = { "acdn", "닉네임" };

        public string Description { get; } = "[RGM] 다른 유저에게 보여지는 이름을 수정합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ApplyChangeCustomInfo : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string args = string.Join(" ", arguments).Trim();

            if (UsersManager.UsersCache.ContainsKey(player.UserId))
            {
                List<string> uc = UsersManager.UsersCache[player.UserId];

                if (int.Parse(uc[2]) >= 100000)
                {
                    uc[6] = args == "" ? "0" : args;
                    UsersManager.UsersCache[player.UserId] = uc;
                    player.CustomInfo = args;

                    response = "인포 변경 완료!\n-";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    response = "해당 기능은 100,000원 후원 혜택입니다.\n-";
                    return false;
                }
            }
            else
            {
                response = "플레이어 정보를 찾을 수 없습니다.\n-";
                return false;
            }
        }

        public string Command { get; } = "applychangecustominfo";

        public string[] Aliases { get; } = { "acci", "인포" };

        public string Description { get; } = "[RGM] 다른 유저에게 보여지는 역할 설명을 수정합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddKillEffect : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            if (arguments.Count < 2)
            {
                response = "플레이어 이름과 킬 이펙트 이름을 입력해주세요.\n-";
                return false;
            }
            else if (RGM.Instance.KillEffects.ContainsKey(args))
            {
                List<string> uc = UsersManager.UsersCache[player.UserId];

                if (uc[3] == "0")
                {
                    uc[3] = args;
                    UsersManager.UsersCache[player.UserId] = uc;
                    response = "킬 이펙트 추가 완료!\n-";

                    UsersManager.SaveUsers();
                    return true;
                }
                else
                {
                    if (uc[3].Contains(args))
                    {
                        response = "이미 해당 킬 이펙트를 보유 중입니다.\n-";
                        return false;
                    }
                    else
                    {
                        uc[3] += $"/{args}";
                        UsersManager.UsersCache[player.UserId] = uc;
                        response = "킬 이펙트 추가 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                }
            }
            else
            {
                response = "존재하지 않는 킬 이펙트 이름입니다.\n-";
                return false;
            }
        }

        public string Command { get; } = "addkilleffect";

        public string[] Aliases { get; } = { "ake" };

        public string Description { get; } = "특정 유저에게 킬 이펙트를 지급합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RemoveKillEffect : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(arguments.At(0));
            string args = string.Join(" ", arguments.Skip(1)).Trim();

            if (arguments.Count < 2)
            {
                response = "플레이어 이름과 킬 이펙트 이름을 입력해주세요.\n-";
                return false;
            }
            else
            {
                List<string> uc = UsersManager.UsersCache[player.UserId];

                if (uc[3] == "0")
                {
                    response = "보유한 킬 이펙트가 없습니다.\n-";
                    return false;
                }
                else
                {
                    if (uc[3].Contains(args))
                    {
                        List<string> Effects = uc[3].Split('/').ToList();

                        Effects.Remove(args);

                        uc[3] = string.Join("/", Effects);
                        if (uc[3] == "") uc[3] = "0";
                        if (uc[4] == args) uc[4] = "0";
                        UsersManager.UsersCache[player.UserId] = uc;
                        response = "킬 이펙트 제거 완료!\n-";

                        UsersManager.SaveUsers();
                        return true;
                    }
                    else
                    {
                        response = "보유한 킬 이펙트에 해당 킬 이펙트가 없습니다.\n-";
                        return false;
                    }
                }
            }
        }

        public string Command { get; } = "removekilleffect";

        public string[] Aliases { get; } = { "rke" };

        public string Description { get; } = "특정 유저가 보유한 킬 이펙트를 제거합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GetPlayerInfo : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(arguments.At(0));

            response = Tools.GetPlayerInfo(player);

            return true;
        }

        public string Command { get; } = "getplayerinfo";

        public string[] Aliases { get; } = { "gpi", "정보조회" };

        public string Description { get; } = "특정 유저의 정보를 조회합니다.";

        public bool SanitizeResponse { get; } = true;
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
                Server.ExecuteCommand($"/cassie_sl <mark=#ffff00aa><color=#000000>운영진(<color=#ffffff>{player.Nickname}</color>)에 의하여 이번 라운드의 모드가 <b>{args}</b>으로 확정되었습니다.</color></mark>");
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
    public class StartMode : ICommand
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
                var modeType = Type.GetType($"RGM.Modes.{RGM.Instance.ModeList[args][2]}");
                if (modeType != null)
                {
                    var modeInstance = Activator.CreateInstance(modeType);
                    var onEnabledMethod = modeType.GetMethod("OnEnabled");
                    onEnabledMethod?.Invoke(modeInstance, null);
                }

                response = $"모드 <b>{args}</b>(을)를 강제로 시작했습니다.\nSending Command Complete!";
                return true;
            }
            else
            {
                response = "존재하지 않는 <모드 이름>입니다.\nSending Command Error..";
                return false;
            }
        }

        public string Command { get; } = "startmode";

        public string[] Aliases { get; } = { "모드시작", "sm" };

        public string Description { get; } = "'/smㅣ모드를 강제로 시작합니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Develop : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            player.Kill("이 자는 개발의 의무를 짊어지고 죽었습니다.");
            player.Role.Set(RoleTypeId.Overwatch);

            response = "Complete!";

            return true;
        }

        public string Command { get; } = "develop";

        public string[] Aliases { get; } = { "dv", "dev", "개발", "roqkf" };

        public string Description { get; } = "개발하러 갈 때 사용하세요!";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddAbility : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Round.IsStarted)
            {
                Player player = Player.Get(arguments.At(0));
                string args = string.Join(" ", arguments).Replace(arguments.At(0), "").Trim();

                if (arguments.Count < 2)
                {
                    RGM.Instance.Requests.Add($"ABattle/{player.Id}/Add/Random");

                    response = "AddAbility Complete!";

                    return true;
                }
                else
                {
                    RGM.Instance.Requests.Add($"ABattle/{player.Id}/Add/{args}");

                    response = "AddAbility Complete!";

                    return true;
                }
            }
            else
            {
                response = "라운드 시작 전에는 사용할 수 없습니다.";

                return false;
            }
        }

        public string Command { get; } = "addability";

        public string[] Aliases { get; } = { "aa", "add" };

        public string Description { get; } = "워크스테이션 업그레이드ㅣ능력을 추가합니다.";

        public bool SanitizeResponse { get; } = true;
    }
}
