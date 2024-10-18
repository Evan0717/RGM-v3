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
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

namespace RGM.Modes
{
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
