using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Server;
using RGM.API.Features;
using Exiled.API.Extensions;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.SCPRUSH)]
    public class SCPRUSH : Mode
    {
        public override string Name => "SCP 러쉬";
        public override string Description => "모든 SCP가 한 개체로 통일됩니다.";
        public override string Detail =>
"""
SCP-3114도 동일한 확률로 러쉬에 참여할 수 있습니다.
""";
        public override string Color => "FE2E2E";

        public static SCPRUSH Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            if (Random.Range(1, 101) <= 10) { //10% 확률로 워크스테이션 업그레이드 시작
                Tools.TryInstallMode(ModeType.ABattle);
            }
            List<RoleTypeId> ScpRoles = new List<RoleTypeId>
            {
                RoleTypeId.Scp049,
                RoleTypeId.Scp096,
                RoleTypeId.Scp106,
                RoleTypeId.Scp173,
                RoleTypeId.Scp939,
                RoleTypeId.Scp079,
                RoleTypeId.Scp3114
            };
            RoleTypeId RandomScpRole = ScpRoles.GetRandomValue();

            foreach (var player in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RandomScpRole))
            {
                player.Role.Set(RandomScpRole);

                if (player.Role is Scp079Role scp079)
                    scp079.Level = 4;
            }

            yield break;
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
