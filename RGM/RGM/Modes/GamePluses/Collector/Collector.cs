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
    public class Collector
    {
        public static Collector Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
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
            List<ItemType> ScpItemList = new List<ItemType>()
            {
                ItemType.SCP018,
                ItemType.SCP268,
                ItemType.SCP207,
                ItemType.SCP500,
                ItemType.SCP1576,
                ItemType.SCP1853,
                ItemType.SCP2176,
                ItemType.SCP244a,
                ItemType.SCP244b,
                ItemType.SCP330,
                ItemType.AntiSCP207
            };


            for (int i=1; i<4; i++)
            {
                Item CurrentItem = player.AddItem(ScpItemList[UnityEngine.Random.Range(0, ScpItemList.Count())]);

                if (player.IsScp)
                {
                    player.CurrentItem = CurrentItem;
                }
            }
        }
    }
}
