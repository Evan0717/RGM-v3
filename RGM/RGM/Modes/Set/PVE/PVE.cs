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
        public override string Author => "A3인데";
        
        
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
            List<Player> players = PlayerManager.List.Where(x => !x.IsNPC).ToList();
            if (players.Count == 0 || roundHandler.SelectedDifficulty < 0)
                return;
            
            int[][] difficultyRewards =
            {
                new int[]{1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 5, 5, 6},
                new int[]{1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 7, 8, 8, 12},
                new int[]{1, 2, 3, 3, 4, 5, 6, 6, 7, 8, 9, 9, 10, 10, 18} 
            };
            
            int reward = difficultyRewards[roundHandler.SelectedDifficulty][roundHandler.CurrentWave];
            if (!roundHandler.AllWavesCleared)
                reward /= 3;

            Timing.RunCoroutine(Tools.SetWinner(players, reward));
        }
    }
}
