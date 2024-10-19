using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.EventArgs
{
    public static class Scp244Events
    {
        public static async void OnUsingScp244(Exiled.Events.EventArgs.Scp244.UsingScp244EventArgs ev)
        {
            await Task.Delay(60 * 1000);

            ev.Scp244.Health = 0;
            ev.Scp244.Destroy();
        }

        public static async void OnOpeningScp244(Exiled.Events.EventArgs.Scp244.OpeningScp244EventArgs ev)
        {
            await Task.Delay(60 * 1000);

            ev.Pickup.Health = 0;
            ev.Pickup.Destroy();
        }
    }
}
