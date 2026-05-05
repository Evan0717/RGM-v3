using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using RGM.API.Features;
using RGM.Modes.PveExiledSystem;

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
