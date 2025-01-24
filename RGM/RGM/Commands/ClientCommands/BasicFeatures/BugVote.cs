using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API;
using RGM.API.Components;
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

using static RGM.Variables.ServerManagers;

namespace RGM.Commands.ClientCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class BugVote : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (IsBugVoteProcessing)
            {
                response = "이미 버그 투표가 진행중입니다.";
                return false;
            }
            else if (BugVoteUsers.Contains(player))
            {
                response = "이미 버그 투표를 한번 사용했습니다.";
                return false;
            }
            else if (arguments.Count < 1)
            {
                response = "이유를 기입해주세요.";
                return false;
            }
            else
            {
                IsBugVoteProcessing = true;

                Timing.RunCoroutine(Tools.BugVote(player, string.Join(" ", arguments)));

                BugVotePlayers.Add(player);
                BugVoteUsers.Add(player);

                response = "버그 투표를 성공적으로 개설하였습니다.";
                return true;
            }
        }

        public string Command { get; } = "버그투표";

        public string[] Aliases { get; } = { "버그", "bugvote", "bug" };

        public string Description { get; } = "[RGM] 게임 진행이 불가능한 경우, 라운드를 강제로 종료하기 위해 사용할 수 있습니다.";

        public bool SanitizeResponse { get; } = true;
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Yes : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!IsBugVoteProcessing)
            {
                response = "버그 투표가 진행중이 아닙니다.";
                return false;
            }
            else if (BugVotePlayers.Contains(player))
            {
                response = "이미 버그 투표에 찬성하였습니다.";
                return false;
            }
            else
            {
                BugVotePlayers.Add(player);

                response = "버그 투표에 찬성하였습니다.";
                return true;
            }
        }

        public string Command { get; } = "찬성";

        public string[] Aliases { get; } = { "yes", "agree" };

        public string Description { get; } = "[RGM] 버그 투표에서 찬성할 때 사용하세요.";

        public bool SanitizeResponse { get; } = true;
    }
}
