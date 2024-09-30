using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using RGM.API;
using UnityEngine;
using Exiled.API.Features.Items;

namespace RGM.Modes
{
    class RandomItem
    {
        public static RandomItem Instance;

        List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in Player.List)
            {
                Spawned(player);
            }

            while (true)
            {
                foreach (var player in Player.List)
                {
                    Item Item = player.AddItem(Tools.GetRandomValue(ItemTypes));

                    if (player.IsScp)
                        player.CurrentItem = Item;
                }

                yield return Timing.WaitForSeconds(60f);
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            player.ClearInventory();

            for (int i = 1; i < 9; i++)
            {
                Item Item = player.AddItem(Tools.GetRandomValue(ItemTypes));

                if (player.IsScp)
                    player.CurrentItem = Item;
            }
        }
    }
}
