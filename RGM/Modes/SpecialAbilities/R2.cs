using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace RGM.Modes.SpecialAbilities
{
    public class R2 // 행운아
    {
        Player target;

        public void OnEnabled(Player player)
        {
            target = player;

            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker += OnInteractingLocker;

            Timing.RunCoroutine(OnStarted());
        }

        public void OnDisabled()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker -= OnInteractingLocker;
        }

        public IEnumerator<float> OnStarted()
        {
            while (target.IsAlive)
                yield return Timing.WaitForSeconds(1f);

            OnDisabled();
        }

        public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Player == target && UnityEngine.Random.Range(0, 100) < 5)
                ev.Door.IsOpen = true;
        }

        public void OnInteractingLocker(Exiled.Events.EventArgs.Player.InteractingLockerEventArgs ev)
        {
            if (ev.Player == target && UnityEngine.Random.Range(0, 100) < 5)
            {
                ev.Chamber.IsOpen = true;
            }
        }
    }
}
