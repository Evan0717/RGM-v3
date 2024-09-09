using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Roles;

namespace RGM.Modes.SpecialAbilities
{
    public class CCTV0 // RTX4090
    {
        Player target;

        public void OnEnabled(Player player)
        {
            target = player;

            Timing.RunCoroutine(OnStarted());
        }

        public IEnumerator<float> OnStarted()
        {
            if (target.Role is Scp079Role scp079)
            {
                scp079.Level = 3;
            }

            yield return 1f;
        }
    }
}
