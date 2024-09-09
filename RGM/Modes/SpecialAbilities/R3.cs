using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace RGM.Modes.SpecialAbilities
{
    public class R3 // 난쟁이
    {
        Player target;

        public void OnEnabled(Player player)
        {
            target = player;

            Timing.RunCoroutine(OnStarted());
        }

        public IEnumerator<float> OnStarted()
        {
            target.Scale = new UnityEngine.Vector3(0.5f, 0.5f, 0.5f);

            yield return 1f;
        }
    }
}
