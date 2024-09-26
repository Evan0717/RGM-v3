using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;

namespace RGM.Modes
{
    public class Nesting
    {
        public static Nesting Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive).ToList())
                {
                    player.MaxHealth = 50000;
                }

                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
