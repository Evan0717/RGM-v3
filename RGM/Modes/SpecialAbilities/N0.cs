using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace RGM.Modes.SpecialAbilities
{
    public class N0 // 재생
    {
        Player target;

        public void OnEnabled(Player player)
        {
            target = player;

            Timing.RunCoroutine(OnStarted());
        }

        public IEnumerator<float> OnStarted()
        {
            while (target.IsAlive)
            {
                if (target.Health < target.MaxHealth)
                    target.Health += 1;

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
