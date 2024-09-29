using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;

namespace RGM.Modes
{
    class RocketLauncher
    {
        public static RocketLauncher Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Hurt += OnHurt;
        }

        public void OnHurt(Exiled.Events.EventArgs.Player.HurtEventArgs ev)
        {
            if (!ev.DamageHandler.IsFriendlyFire && ev.Player != ev.Attacker)
            {
                if (UnityEngine.Random.Range(1, 21) == 1)
                {
                    Server.ExecuteCommand($"/cassie_sl <color={ev.Player.Role.Color.ToHex()}>{ev.Player.Role.Name}</color>(이)가 하늘로 승천했습니다.");
                    Server.ExecuteCommand($"/rocket {ev.Player.Id} 1");
                }
            }
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return 0f;
        }
    }
}
