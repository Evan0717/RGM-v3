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
using RGM.Discord;
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
            else
            {
                ChatCooldown.Add(player);

                string ColorFormat(string cn)
                {
                    if (ColorUtility.TryParseHtmlString(cn, out Color color))
                        return color.ToHex();

                    else
                    {
                        var cd = Datas.Colors;

                        if (cd.ContainsKey(cn))
                            return cd[cn];

                        else
                            return "#FFFFFF";
                    }
                }

                string BadgeFormat(Player player)
                {
                    if (player.Group != null && !player.BadgeHidden)
                        return $"[<color={ColorFormat(player.Group.BadgeColor)}>{player.Group.BadgeText}</color>] ";

                    else
                        return "";
                }

                string ChatFormat(string chatType)
                {
                    string text = Trans.Role[player.Role.Type];
                    string text2 = string.Concat(new string[]
                    {
                        $"<size=25>{BadgeFormat(player)}<color={player.Role.Color.ToHex()}>",
                        text,
                        $"</color> ({player.DisplayNickname}) <b> | </b>",
                        string.Join(" ", arguments).Replace("=", "❤️"),
                        "</size>"
                    });

                    bool Check(Player p)
                    {
                        if (player.VoiceChannel == VoiceChat.VoiceChatChannel.Intercom)
                            return true;

                        if (p.CurrentItem is Scp1576 scp1576)
                        {
                            if (scp1576.IsUsing)
                                return true;
                        }

                        if (chatType == "SCP 채팅")
                            return p.IsScp || p.IsDead;

                        else if (chatType == "관전자 채팅")
                            return p.IsDead;

                        else if (chatType == "근거리 채팅")
                            return Vector3.Distance(p.Position, p.Position) <= 10 || p.IsDead;
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

                if (player.IsScp)
                {
                    response = ChatFormat("SCP 채팅");
                    return true;
                }
                else if (player.IsDead)
                {
                    response = ChatFormat("관전자 채팅");
                    return true;
                }
                else
                {
                    response = ChatFormat("근거리 채팅");
                    return true;
                }
            }
        }

        public string Command { get; } = "chat";
        public string[] Aliases { get; } = new string[] { "챗", "채팅", "ㅊ", "c" };
        public string Description { get; } = "[RGM] 텍스트 채팅을 사용할 수 있습니다.";
    }
}
