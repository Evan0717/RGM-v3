using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace RGM.Modes.SpecialAbilities
{
    public class N2 // 벌크업
    {
        Player target;

        public void OnEnabled(Player player)
        {
            target = player;

            Exiled.Events.Handlers.Player.Hurting += OnHurting;

            Timing.RunCoroutine(OnStarted());
        }

        public void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        public IEnumerator<float> OnStarted()
        {
            while (target.IsAlive)
                yield return Timing.WaitForSeconds(1f);

            OnDisabled();
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker == target)
                ev.DamageHandler.Damage = ev.DamageHandler.Damage + ev.DamageHandler.Damage * 0.2f;
        }
    }
}
