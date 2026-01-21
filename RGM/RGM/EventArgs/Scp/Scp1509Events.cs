using Exiled.Events.EventArgs.Scp1509;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.EventArgs
{
    public static class Scp1509Events
    {
        public static void OnResurrecting(ResurrectingEventArgs ev)
        {
            if (ev.Player.IsScpRole())
            {
                ev.IsAllowed = false;

                Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                {
                    if (ev.Player.IsScp)
                        ev.Victim.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.None);

                    else
                        ev.Victim.Role.Set(ev.Player.Role.Type, RoleSpawnFlags.None);
                });
            }
        }
    }
}
