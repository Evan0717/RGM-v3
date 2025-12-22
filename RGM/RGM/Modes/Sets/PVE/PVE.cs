using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using Mirror;
using MultiBroadcast;
using MultiBroadcast.API;
using PlayerRoles;
using Respawning;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.PVE)]
    class PVE : Mode
    {
        public override string Name => "공성전";
        public override string Description => "AI들의 웨이브를 버텨내세요.";
        public override string Detail =>
"""
나도이게뭔지잘몰?루
""";
        public override string Color => "a0aade";
        public override string Suggester => "made by A3인데(@a3ind)";

        RoundHandler roundHandler;

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.PauseWaves(); 

            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            roundHandler = new RoundHandler();
            roundHandler.OnRoundStarted();

            Exiled.Events.Handlers.Server.EndingRound += roundHandler.OnEndingRound;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            roundHandler.OnEndingRound();
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

            if (players.Count() == 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

            else if (players.Count() > 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
        }
    }
}
