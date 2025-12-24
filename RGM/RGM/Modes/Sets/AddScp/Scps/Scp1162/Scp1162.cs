using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using RGM.API.Features;
using RGM.API.DataBases;
using UnityEngine;
using Exiled.API.Features.Items;
using PlayerRoles;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using Exiled.API.Features.Pickups;
using Exiled.API.Enums;
using static PlayerList;
using RemoteAdmin;

namespace RGM.Modes.Sets.AddScp.Scps
{
    public static class Scp1162
    {
        public static void OnEnabled()
        {
            Exiled.Events.Handlers.Player.DroppedItem += OnDroppedItem;

            Timing.RunCoroutine(OnStarted());
        }

        public static IEnumerator<float> OnStarted()
        {
            Tools.LoadMap("Scp1162");

            yield return 0;
        }

        public static void OnDroppedItem(DroppedItemEventArgs ev)
        {
            if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 2))
            {
                if (hit.transform.name == "Scp1162")
                {
                    List<ItemType> ItemList = Tools.EnumToList<ItemType>();
                    ItemType Item = Tools.GetRandomValue(ItemList);

                    int rand = UnityEngine.Random.Range(1, 101);

                    if (0 < rand && rand < 3)
                    {
                        ev.Player.EnableEffect(EffectType.SeveredHands, 1, 50);
                    }
                    else
                    {
                        ev.Pickup.Destroy();
                        Item CurrentItem = ev.Player.AddItem(Item);
                        ev.Player.DropItem(CurrentItem);
                    }
                }
            }
        }
    }
}
