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
            foreach (var player in Player.List)
            {
                if (GodModePlayers.Contains(player))
                    GodModePlayers.Remove(player);
            }
        }
    }
}
