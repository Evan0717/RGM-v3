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
    public class InfiniteAmmo
    {
        public static InfiniteAmmo Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.UsingMicroHIDEnergy += OnUsingMicroHIDEnergy;
            Exiled.Events.Handlers.Item.Swinging += OnSwinging;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield break;
        }

        public void OnShooting(Exiled.Events.EventArgs.Player.ShootingEventArgs ev)
        {
            ev.Firearm.Ammo = ev.Firearm.MaxAmmo;
        }

        public void OnUsingMicroHIDEnergy(Exiled.Events.EventArgs.Player.UsingMicroHIDEnergyEventArgs ev)
        {
            ev.MicroHID.Energy = 100f;
        }

        public void OnSwinging(Exiled.Events.EventArgs.Item.SwingingEventArgs ev)
        {
            ev.Item.As<Exiled.API.Features.Items.Jailbird>().TotalCharges = 3;
        }
    }
}
