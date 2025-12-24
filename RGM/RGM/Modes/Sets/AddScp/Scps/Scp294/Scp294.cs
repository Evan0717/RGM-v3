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
    public static class Scp294
    {
        static Dictionary<ItemType, float> vandingMachineChances { get; set; } = new()
        {
            { ItemType.SCP330, 85 },
            { ItemType.SCP207, 5 },
            { ItemType.Jailbird, 5 },
            { ItemType.AntiSCP207, 4 },
            { ItemType.MicroHID, 1 }
        };
        
        static IEnumerator<float> start(Player player, Transform transform)
        {
            Transform sound = transform.parent.parent.GetChild(3).GetChild(2);

            if (player.CurrentItem != null && player.CurrentItem.Type == ItemType.Coin)
            {
                Tools.PlaySound(sound, "vm_insert_success", 2);

                player.RemoveItem(player.CurrentItem);

                Transform output = transform.parent.parent.GetChild(3).GetChild(0);

                List<ItemType> list = new();

                foreach (var chance in vandingMachineChances)
                {
                    for (int i = 0; i < chance.Value; i++)
                    {
                        list.Add(chance.Key);
                    }
                }

                yield return Timing.WaitForSeconds(0.5f);

                Tools.PlaySound(output, "vm_drop", 2);

                yield return Timing.WaitForSeconds(0.5f);

                ItemType itemType = list.GetRandomValue();

                if (player.IsScpRole())
                {
                    player.AddItem(itemType);
                }
                else
                {
                    if (itemType == ItemType.SCP330)
                    {
                        Scp330 scp330 = (Scp330)Item.Create(ItemType.SCP330);
                        scp330.AddCandy(Tools.EnumToList<CandyKindID>().GetRandomValue());
                        scp330.RemoveCandy(scp330.Candies.ToList()[0]);

                        scp330.CreatePickup(output.position);
                    }
                    else
                    {
                        Pickup.CreateAndSpawn(itemType, output.position);
                    }
                }
            }
            else
            {
                Tools.PlaySound(sound, "vm_insert_fail", 2);
            }
        }

        public static void OnEnabled()
        {
            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;

            Timing.RunCoroutine(OnStarted());
        }

        public static IEnumerator<float> OnStarted()
        {
            Tools.LoadMap("vm");

            foreach (var pickup in Pickup.List.ToList())
            {
                try
                {
                    if (UnityEngine.Random.Range(1, 11) == 1)
                    {
                        Item coin = Item.Create(ItemType.Coin);
                        coin.CreatePickup(pickup.Transform.position + new Vector3(0, 0.1f, 0));
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"[PlanB] Error while creating coin: {e}");
                }
            }

            yield return 0;
        }

        static void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (ev.Pickup.Transform.name.StartsWith("Input"))
            {
                ev.IsAllowed = false;

                Timing.RunCoroutine(start(ev.Player, ev.Pickup.Transform));
            }
        }

        static void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 10))
            {
                var transform = hit.transform;

                if (transform.name.StartsWith("Input"))
                {
                    ev.IsAllowed = false;

                    Timing.RunCoroutine(start(ev.Player, transform));
                }
            }
        }

        static void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 10))
            {
                var transform = hit.transform;

                if (transform.name.StartsWith("Input"))
                {
                    ev.IsAllowed = false;

                    Timing.RunCoroutine(start(ev.Player, transform));
                }
            }
        }
    }
}
