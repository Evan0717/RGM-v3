using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.EventArgs
{
    public static class Scp244Events
    {
        public static IEnumerator<float> OnUsingScp244(Exiled.Events.EventArgs.Scp244.UsingScp244EventArgs ev)
        {
            yield return Timing.WaitForSeconds(60);

            ev.Scp244.Health = 0;
            ev.Scp244.Destroy();
        }

        public static IEnumerator<float> OnOpeningScp244(Exiled.Events.EventArgs.Scp244.OpeningScp244EventArgs ev)
        {
            yield return Timing.WaitForSeconds(60);

            ev.Pickup.Health = 0;
            ev.Pickup.Destroy();
        }
    }
}
