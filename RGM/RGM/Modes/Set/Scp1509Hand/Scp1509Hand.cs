using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using Mirror;
using PlayerRoles;
using RGM.API.DataBases;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.Scp1509Hand)]
    class Scp1509Hand : Mode
    {
        public override string Name => "마체테 클로";
        public override string Description => "상대방을 처치하면 즉시 자신의 팀으로 만듭니다.";
        public override string Detail =>
"""
인간이 처치할 경우, 같은 진영으로 변경됩니다.

SCP가 처치할 경우, SCP-049-2로 변경됩니다.
""";
        public override string Color => "7a7a7a";

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Died -= OnDied;
        }

        void OnDied(DiedEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (ev.Attacker.IsScp)
                    ev.Player.Role.Set(RoleTypeId.Scp0492);

                else
                    ev.Player.Role.Set(ev.Attacker.Role.Type);
            }
        }
    }
}
