using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Enums;
using MEC;
using Exiled.API.Features.Roles;

namespace RGM.Modes
{
    public class KoreanSpeed
    {
        public static KoreanSpeed Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            while (true)
            {
                foreach (var player in Player.List)
                {
                    player.EnableEffect(EffectType.MovementBoost, 125);
                    player.EnableEffect(EffectType.Scp1853, 5);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
