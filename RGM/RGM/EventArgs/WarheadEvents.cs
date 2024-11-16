using Exiled.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

using static RGM.Variables.ServerManagers;

namespace RGM.EventArgs
{
    public static class WarheadEvents
    {
        public static void OnStopping(Exiled.Events.EventArgs.Warhead.StoppingEventArgs ev)
        {
            if (AutoNuke)
                ev.IsAllowed = false;
        }

        public static void OnDetonating(Exiled.Events.EventArgs.Warhead.DetonatingEventArgs ev)
        {
            Player.List.Where(x => x.Zone != ZoneType.Surface && x.IsAlive).ToList().ForEach(x =>
            {
                if (GodModePlayers.Contains(x))
                    GodModePlayers.Remove(x);

                x.Kill("핵폭발에 사망하였습니다.");
            });
        }
    }
}
