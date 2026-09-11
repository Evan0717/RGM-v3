using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;

using PlayerRoles;
using RGM.API.Features;
using RGM.API.DataBases;
using UnityEngine;

using static RGM.Variables.Variable;
using DiscordInteraction.Discord;
using Exiled.API.Features.Items;
using MEC;
using RGM.Modes.SubClass;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Chat : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            string args = string.Join(" ", arguments);

            if (args == "")
            {
                response =  "보낼 메세지를 입력해주세요.";
                return false;
            }

            if (ChatCooldown.Contains(player))
            {
                response =  "너무 빠른 간격으로 입력을 보내고 있습니다!";
                return false;
            }

            if (player.IsMuted)
            {
                response =  "뮤트된 상태입니다.";
                return false;
            }

            if (SelectMode.Contains("Secret") && Round.IsLobby)
            {
                response =  "비밀 선거 조항을 깨트리지 마십시오.";
                return false;
            }

            if (EnabledModeList.Select(x => x.Data.Type).Contains(ModeType.Silent))
            {
                if (player.IsAlive)
                {
                    var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, player);
                    g.FuseTime = 0;
                    g.BurnDuration = 0;
                    g.MaxRadius = 0;
                    g.SpawnActive(player.Position);

                    if (GodModePlayers.Contains(player))
                        GodModePlayers.Remove(player);

                    player.Kill( "입이 근질거리는 것을 참지 못했습니다.");

                    response =  "쉿!";
                    return false;
                }
            }

            ChatCooldown.Add(player);

            string ChatFormat(string chatType)
            {
                string text =  Trans.Role[player.Role.Type];
                string prefix = string.Concat(new string[]
                {
                    $"<size=20><b>{chatType}</b>ㅣ{Tools.BadgeFormat(player)}<color={player.Role.Color.ToHex()}>",
                    text,
                    $"</color> (<b><i>{player.DisplayNickname}</i></b>) <b> | </b>",
                });

                string suffix = "</size>";

                string rawMessage = args.Replace("</noparse>", "");
                string rawMessageNoParse = $"<noparse>{rawMessage}</noparse>";

                string text2 = $"{prefix}{rawMessageNoParse}{suffix}";

                Chats[player].Add(args);

                Tools.PlaySound(player.Transform, $"SCP Message Pop {UnityEngine.Random.Range(1, 3)}", 3f);

                Timing.CallDelayed(10, () =>
                {
                    Chats[player].Remove(args);
                });

                bool Check(Player p)
                {
                    if (chatType == "전체")
                        return true;

                    if (chatType == "SCP")
                        return p.IsDead || NonePlayer.Players.Contains(p) || p.IsScp || p.Role.Type == RoleTypeId.ZombieFlamingo;

                    if (chatType == "플라밍고")
                        return p.IsDead || NonePlayer.Players.Contains(p) || new List<RoleTypeId>() { RoleTypeId.Flamingo, RoleTypeId.AlphaFlamingo }.Contains(p.Role.Type);

                    if (chatType == "관전자")
                        return p.IsDead || NonePlayer.Players.Contains(p);

                    if (chatType == "SCP-1576 + 무전기")
                        return p.IsDead || NonePlayer.Players.Contains(p) || Vector3.Distance(p.Position, player.Position) <= 10 || p.HasItem(ItemType.Radio);

                    if (chatType == "SCP-1576")
                        return p.IsDead || NonePlayer.Players.Contains(p) || Vector3.Distance(p.Position, player.Position) <= 10;

                    if (chatType == "무전기")
                        return p.IsDead || NonePlayer.Players.Contains(p) || Vector3.Distance(p.Position, player.Position) <= 10 || p.HasItem(ItemType.Radio);

                    if (chatType == "근거리")
                        return p.IsDead || NonePlayer.Players.Contains(p) || Vector3.Distance(p.Position, player.Position) <= 10;

                    return false;
                }

                foreach (Player ply in PlayerManager.List.Where(x => !x.IsNPC)) // PlayerManager를 쓰지 않는 이유: 훈련장의 인원들도 듣긴 들어야지... 하지만 훈련장 삭제했죠? 다시 쓰기
                {
                    if (Check(ply))
                    {
                        string msg = $"{prefix}<noparse>{rawMessage.Replace("</noparse>", "")}</noparse>{suffix}";
                        ply.AddBroadcast(6, msg, tag: "chat");
                    }
                }

                Webhook.Send($"**{chatType}**ㅣ`<b><i>{player.DisplayNickname}</i></b>`[{player.IPAddress}, {player.UserId}]({( Trans.Role[player.Role.Type])}) - {string.Join(" ", arguments)}");

                return $"'{text2}'";
            }

            if (arguments.Count == 0)
            {
                response = "보낼 메세지를 입력해주세요.";
                return false;
            }

            if (IntercomPlayers.Contains(player))
            {
                response = ChatFormat("전체");
                return true;
            }
                
            if (player.IsScp || player.Role.Type == RoleTypeId.ZombieFlamingo)
            {
                response = ChatFormat("SCP");
                return true;
            }

            if (new List<RoleTypeId>() { RoleTypeId.Flamingo, RoleTypeId.AlphaFlamingo }.Contains(player.Role.Type))
            {
                response = ChatFormat("플라밍고");
                return true;
            }

            if (player.IsDead || NonePlayer.Players.Contains(player))
            {
                response = ChatFormat("관전자");
                return true;
            }
                
            if (player.CurrentItem is Scp1576 { IsUsing: true })
            {
                if (player.HasItem(ItemType.Radio))
                {
                    response = ChatFormat("SCP-1576 + 무전기");
                    return true;
                }

                response = ChatFormat("SCP-1576");
                return true;
            }
                
            if (player.HasItem(ItemType.Radio))
            {
                response = ChatFormat("무전기");
                return true;
            }

            response = ChatFormat("근거리");
            return true;
        }

        public string Command { get; } = "c";
        public string[] Aliases { get; } = ["챗", "채팅", "chat", "ㅊ"];
        public string Description { get; } =  "[RGM] 텍스트 채팅을 사용할 수 있습니다.";
    }
}
