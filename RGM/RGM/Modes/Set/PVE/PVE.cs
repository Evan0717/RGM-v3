using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using RGM.API.Features;
using RGM.Modes.PveExiledSystem;
using RGM.Variables;

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
        public override string Author => "A3인데";


        RoundHandler roundHandler;

        public override void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.PauseWaves(); 

            Exiled.Events.Handlers.Server.EndingRound += OnRoundEnding;
            Exiled.Events.Handlers.Server.EndingRound += roundHandler.OnEndingRound;

            roundHandler = new RoundHandler();
            roundHandler.OnRoundStarted();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.EndingRound -= OnRoundEnding;
            Exiled.Events.Handlers.Server.EndingRound -= roundHandler.OnEndingRound;

            roundHandler.OnEndingRound();
        }

        public void OnRoundEnding(EndingRoundEventArgs ev)
        {
            List<Player> players = PlayerManager.List.Where(x => !x.IsNPC).ToList();
            if (players.Count == 0 || roundHandler.SelectedDifficulty < 0)
                return;
            

            int reward = -1;
            reward = roundHandler.SelectedDifficulty switch
            {
                0 => roundHandler.CurrentWave switch
                {
                    <= 4 => 1,
                    <= 7 => 2,
                    <= 10 => 3,
                    <= 12 => 4,
                    <= 14 => 5,
                    15 => 6,
                    _ => reward
                },
                1 => roundHandler.CurrentWave switch
                {
                  <= 2 => 1,
                  <= 4 => 2,
                  <= 6 => 3,
                  <= 8 => 4,
                  <= 10 => 5,
                  11 => 6,
                  12 => 7,
                  <= 14 => 8,
                  15 => 12,
                  _ => reward
                },
                2 => roundHandler.CurrentWave switch
                {
                    <= 2 => 1,
                    3 => 2,
                    <= 5 => 3,
                    6 => 4,
                    <= 8 => 5,
                    9 => 6,
                    <= 11 => 7,
                    <= 13 => 8,
                    <= 15 => 9,
                    16 => 18,
                    _ => reward
                },
                _ => reward
            };

            List<Player> wonplayers =
            [
                .. players
                    .Where(p => Variable.PlayersReport.TryGetValue(p.UserId, out var report)
                                && report.Damage >= 0)
            ];

            reward -= roundHandler.AllWavesCleared ? 1 : 0;
            reward = reward <= 0 ? 5 : reward;
            Timing.RunCoroutine(Tools.SetWinner(wonplayers, reward));
        }
    }
}
