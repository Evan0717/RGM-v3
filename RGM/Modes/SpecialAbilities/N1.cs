using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace RGM.Modes.SpecialAbilities
{
    public class N1 // 운동
    {
        Player target;

        public void OnEnabled(Player player)
        {
            target = player;

            Timing.RunCoroutine(OnStarted());
        }

        public IEnumerator<float> OnStarted()
        {
            target.MaxHealth = target.MaxHealth * 1.5f;
            target.Health = target.MaxHealth;

            yield return 1f;
        }
    }
}
