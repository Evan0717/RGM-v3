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
    public class JohnWick
    {
        public static JohnWick Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield break;
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            List<ItemType> Pistols = new List<ItemType>()
            {
                ItemType.GunCOM15,
                ItemType.GunCOM18,
                ItemType.GunCom45,
                ItemType.GunRevolver
            };

            if (Pistols.Contains(ev.Attacker.CurrentItem.Type))
            {
                ev.DamageHandler.Damage = 4 * ev.DamageHandler.Damage;
            }
        }
    }
}
