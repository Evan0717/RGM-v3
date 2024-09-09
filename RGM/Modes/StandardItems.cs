using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Exiled.CreditTags.Features;
using RGM.API;

namespace RGM.Modes
{
    public class StandardItems
    {
        public static StandardItems Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

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

        public async void Spawned(Player player)
        {
            player.ClearInventory();

            await Task.Delay(100);

            List<ItemType> itemList = Tools.EnumToList<ItemType>();

            for (int i=1; i<UnityEngine.Random.Range(6, 10); i++)
            {
                ItemType toGive = itemList[UnityEngine.Random.Range(0, itemList.Count())];
                Item CurrentItem = player.AddItem(toGive);

                if (player.IsScp)
                {
                    player.CurrentItem = CurrentItem;
                }
            }
        }
    }
}
