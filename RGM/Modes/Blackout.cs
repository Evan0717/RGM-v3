using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using MEC;

namespace RGM.Modes
{
    public class Blackout
    {
        public static Blackout Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var room in Room.List)
            {
                switch (UnityEngine.Random.Range(1, 4))
                {
                    case 1:
                        room.TurnOffLights();
                        break;
                    case 2:
                        room.Color = Color.HSVToRGB(UnityEngine.Random.Range(1f, 255f), UnityEngine.Random.Range(1f, 255f), UnityEngine.Random.Range(1f, 255f));
                        break;
                    case 3:
                        break;
                }
            }

            foreach (var player in Player.List)
            {
                Spawned(player);
            }

            yield break;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (UnityEngine.Random.Range(1, 11) == 1)
                player.AddItem(ItemType.Lantern);

            else
                player.AddItem(ItemType.Flashlight);
        }
    }
}