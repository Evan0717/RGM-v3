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

namespace RGM.Commands.ClientCommands
{
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

                string ColorFormat(string cn)
                {
                    if (ColorUtility.TryParseHtmlString(cn, out Color color))
                        return color.ToHex();

                    else
                    {
                        var cd = Tools.GetColorsDictionary();

                        if (cd.ContainsKey(cn))
                            return cd[cn];

                        else
                            return "#FFFFFF";
                    }
                }

                string BadgeFormat(Player player)
                {
                    if (player.Group != null)
                        return $"[<color={ColorFormat(player.Group.BadgeColor)}>{player.Group.BadgeText}</color>] ";

                    else
                        return "";
                }

                if (player.IsScp)
                {
                    if (arguments.Count == 0)
                    {
                        response = "보낼 메세지를 입력해주세요.";
                        return false;
                    }

                    string text = Trans.Role[player.Role.Type];
                    string text2 = string.Concat(new string[]
                    {
                        $"<size=25>{BadgeFormat(player)}<color={player.Role.Color.ToHex()}>",
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
                    string text = Trans.Role[player.Role.Type];
                    string text2 = string.Concat(new string[]
                    {
                        $"<size=25>{BadgeFormat(player)}<color={player.Role.Color.ToHex()}>",
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
                    string text = Trans.Role[player.Role.Type];
                    string text2 = string.Concat(new string[]
                    {
                        $"<size=25>{BadgeFormat(player)}<color={player.Role.Color.ToHex()}>",
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
}
