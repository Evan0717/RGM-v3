using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using Mirror;
using PlayerRoles;
using RGM.API.DataBases;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.ChupaChups)]
    class ChupaChups : Mode
    {
        public override string Name => "츄파춥스";
        public override string Description => "Jailbird.. 요즘 하나씩은 다 가지고 있죠?";
        public override string Detail =>
"""
스폰하면 즉시 Jailbird를 얻습니다.
""";
        public override string Color => "2ECCFA";

        public static ChupaChups Instance;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= OnRespawningTeam;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in PlayerManager.List)
            {
                Timing.RunCoroutine(Spawned(player));
            }

            yield break;
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Timing.RunCoroutine(Spawned(ev.Player));
        }

        public IEnumerator<float> Spawned(Player player)
        {
            yield return Timing.WaitForSeconds(1);

            player.AddItem(ItemType.Jailbird);
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            foreach (var player in PlayerManager.List.Where(x => x.IsAlive && x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
                Spawned(player);
        }
    }
}
