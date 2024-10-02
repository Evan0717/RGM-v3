using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
using RGM.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerStatsSystem;

namespace RGM.Donator
{
    public class Main
    {
        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            List<string> Attacker = DonatorsManager.UsersCache[ev.Attacker.UserId];
            List<string> Player = DonatorsManager.UsersCache[ev.Player.UserId];

            if (ev.Attacker != null && Attacker[0] != "0")
            {
                if (Attacker[0] == "영혼 가출")
                {
                    DamageHandlerBase Disruptor = new DisruptorDamageHandler(ev.Attacker.Footprint, -1);

                    Ragdoll.CreateAndSpawn(ev.TargetOldRole.GetRoleBase().RoleTypeId, Attacker[0], Disruptor, ev.Player.Position, ev.Player.Rotation);
                }
            }
        }
    }
}
