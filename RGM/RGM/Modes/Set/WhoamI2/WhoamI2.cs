using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.API.Extensions;
using RGM.Patches;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.WhoamI2)]
    public class WhoamI2 : Mode
    {
        public override string Name => "누구게?";
        public override string Description => "1분마다 진영이 변경됩니다.";
        public override string Detail =>
"""
1분마다 진영이 변경된다는 설명으로도 충분하다.

* 게임 시작 10분 뒤 <color=red>자동핵</color>이 작동됩니다.
""";
        public override string Color => "01DF74";

        public static WhoamI2 Instance;

        List<RoleTypeId> ignoredRoles = new List<RoleTypeId>
        {
            RoleTypeId.Scp079,
            RoleTypeId.Spectator,
            RoleTypeId.Overwatch,
            RoleTypeId.Filmmaker,
            RoleTypeId.None,
            RoleTypeId.Flamingo,
            RoleTypeId.AlphaFlamingo,
            RoleTypeId.ZombieFlamingo,
            RoleTypeId.NtfFlamingo,
            RoleTypeId.ChaosFlamingo,
            RoleTypeId.Destroyed,
            RoleTypeId.CustomRole
        };

        private CoroutineHandle _onModeStarted;
        private readonly AutoWarhead _autoWarhead = new(10, 1);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _autoWarhead.RunCoroutine();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Timing.KillCoroutines(_onModeStarted);
            _autoWarhead.KillCoroutine();
        }

        private IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(60f);

                foreach (var player in PlayerManager.List.Where(x => !ignoredRoles.Contains(x.Role.Type)).ToList())
                    player.Role.Set(Tools.EnumToList<RoleTypeId>().Where(x => !ignoredRoles.Contains(x)).ToList().GetRandomValue(), RoleSpawnFlags.None);
            }
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            IList<Player> players = [.. PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC)];

            switch (players.Count)
            {
                case 1:
                    Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));
                    break;
                case > 1:
                    Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
                    break;
            }
        }
    }
}
