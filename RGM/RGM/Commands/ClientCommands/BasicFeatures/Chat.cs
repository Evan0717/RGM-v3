using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API.Features;
using RGM.API.DataBases;
using RGM.Modes;
using UnityEngine;

using static RGM.Variables.ServerManagers;
using DiscordInteraction.Discord;

using CustomPlayerEffects;
using Exiled.API.Features.Items;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Chat : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (ChatCooldown.Contains(player))
            {
                response = "너무 빠른 간격으로 입력을 보내고 있습니다!";
                return false;
            }
            else if (player.IsMuted)
            {
                response = "뮤트된 상태입니다.";
                return false;
            }
            else if (SelectMode == "SecretVote" && Round.IsLobby)
            {
                response = "비밀 선거 조항을 깨트리지 마십시오.";
                return false;
            }
            else if (EnabledModeList.Contains(ModeType.Silent))
            {
                if (player.IsAlive)
                {
                    var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, player);
                    g.FuseTime = 0f;
                    g.BurnDuration = 0f;
                    g.SpawnActive(player.Position);

                    if (GodModePlayers.Contains(player))
                        GodModePlayers.Remove(player);

                    player.Kill("입이 근질거리는 것을 참지 못했습니다.");

                    response = "쉿!";
                    return false;
                }
            }

            ChatCooldown.Add(player);

            string ChatFormat(string chatType)
            {
                string text = Trans.Role[player.Role.Type];
                string text2 = string.Concat(new string[]
                {
                    $"<size=25><b>{chatType}</b>ㅣ{Tools.BadgeFormat(player)}<color={player.Role.Color.ToHex()}>",
                    text,
                    $"</color> ({player.DisplayNickname}) <b> | </b>",
                    string.Join(" ", arguments).Replace("=", "❤️"),
                    "</size>"
                });

                bool Check(Player p)
                {
                    if (chatType == "전체")
                        return true;

                    if (chatType == "SCP")
                        return p.IsDead || p.IsScp || p.Role.Type == RoleTypeId.ZombieFlamingo;

                    if (chatType == "플라밍고")
                        return p.IsDead || new List<RoleTypeId>() { RoleTypeId.Flamingo, RoleTypeId.AlphaFlamingo }.Contains(p.Role.Type);

                    if (chatType == "관전자")
                        return p.IsDead;

                    if (chatType == "SCP-1576 + 무전기")
                        return p.IsDead || Vector3.Distance(p.Position, player.Position) <= 10 || p.HasItem(ItemType.Radio);

                    if (chatType == "SCP-1576")
                        return p.IsDead || Vector3.Distance(p.Position, player.Position) <= 10;

                    if (chatType == "무전기")
                        return p.IsDead || Vector3.Distance(p.Position, player.Position) <= 10 || p.HasItem(ItemType.Radio);

                    if (chatType == "근거리")
                        return p.IsDead || Vector3.Distance(p.Position, player.Position) <= 10;

                    return false;
                }

                foreach (Player ply in Player.List)
                {
                    if (Check(ply))
                        ply.AddBroadcast(6, text2);
                }

                Webhook.Send($"**{chatType}**ㅣ`{player.DisplayNickname}`[{player.IPAddress}, {player.UserId}]({Trans.Role[player.Role.Type]}) - {string.Join(" ", arguments)}");

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
                
            if (player.IsDead)
            {
                response = ChatFormat("관전자");
                return true;
            }
                
            if (player.CurrentItem is Scp1576 scp1576)
            {
                if (scp1576.IsUsing)
                {
                    if (player.HasItem(ItemType.Radio))
                    {
                        response = ChatFormat("SCP-1576 + 무전기");
                        return true;
                    }
                    else
                    {
                        response = ChatFormat("SCP-1576");
                        return true;
                    }
                }
            }
                
            if (player.HasItem(ItemType.Radio))
            {
                response = ChatFormat("무전기");
                return true;
            }

            response = ChatFormat("근거리");
            return true;
        }

        public string Command { get; } = "ㅊ";
        public string[] Aliases { get; } = new string[] { "챗", "채팅", "chat", "c" };
        public string Description { get; } = "[RGM] 텍스트 채팅을 사용할 수 있습니다.";
    }
}
