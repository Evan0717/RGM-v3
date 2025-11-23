using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Scp330;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using MultiBroadcast.API;
using RGM.API.Features;
using RGM.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.EventArgs
{
    public static class Scp330Events
    {
        public static void OnInteractingScp330(InteractingScp330EventArgs ev)
        {
            if (ev.Player.IsScp)
                return;

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                if (UnityEngine.Random.Range(1, 6) == 1)
                {
                    ev.Player.TryRemoveCandу(ev.Candy);
                    ev.Player.AddCandy(Tools.EnumToList<CandyKindID>().GetRandomValue());
                }
            });
        }
    }
}
