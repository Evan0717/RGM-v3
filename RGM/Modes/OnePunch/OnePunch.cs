using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.Modes
{
    public class OnePunch
    {
        public static OnePunch Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            ev.DamageHandler.Damage = 12050;
        }
    }
}
