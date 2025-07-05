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

        public static void OnEnabled()
        {
            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;

            Timing.RunCoroutine(OnStarted());

            CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(new SetScp294());
        }

        public static IEnumerator<float> OnStarted()
        {
            Tools.LoadMap("vm");

            foreach (var pickup in Pickup.List)
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

        public static IEnumerator<float> OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (ev.Pickup.Transform.name.StartsWith("Input"))
            {
                ev.IsAllowed = false;

                Transform sound = ev.Pickup.Transform.parent.parent.GetChild(3).GetChild(2);

                if (ev.Player.CurrentItem != null && ev.Player.CurrentItem.Type == ItemType.Coin)
                {
                    Tools.PlaySound(sound, "vm_insert_success", 2);

                    ev.Player.RemoveItem(ev.Player.CurrentItem);

                    Transform output = ev.Pickup.Transform.parent.parent.GetChild(3).GetChild(0);

                    if (ev.Player.IsScp)
                    {
                        yield return Timing.WaitForSeconds(0.5f);

                        Tools.PlaySound(output, "vm_drop", 2);

                        yield return Timing.WaitForSeconds(0.5f);

                        List<EffectType> effects = new()
                        {
                            EffectType.MovementBoost,
                            EffectType.Invisible,
                            EffectType.AntiScp207,
                            EffectType.Invigorated,
                            EffectType.DamageReduction,
                        };

                        ev.Player.AddEffect
                        (
                            effects.GetRandomValue(), 
                            (byte)UnityEngine.Random.Range(1, UnityEngine.Random.Range(10, 256)), 
                            UnityEngine.Random.Range(1, UnityEngine.Random.Range(10, 61)), 
                            true
                        );
                    }
                    else
                    {
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
        }

        static IEnumerator<float> OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 10))
            {
                var transform = hit.transform;

                if (transform.name.StartsWith("Input"))
                {
                    ev.IsAllowed = false;

                    Transform sound = transform.parent.parent.GetChild(3).GetChild(2);

                    if (ev.Player.CurrentItem != null && ev.Player.CurrentItem.Type == ItemType.Coin)
                    {
                        Tools.PlaySound(sound, "vm_insert_success", 2);

                        ev.Player.RemoveItem(ev.Player.CurrentItem);

                        Transform output = transform.parent.parent.GetChild(3).GetChild(0);

                        if (ev.Player.IsScp)
                        {
                            yield return Timing.WaitForSeconds(0.5f);

                            Tools.PlaySound(output, "vm_drop", 2);

                            yield return Timing.WaitForSeconds(0.5f);

                            List<EffectType> effects = new()
                        {
                            EffectType.MovementBoost,
                            EffectType.Invisible,
                            EffectType.AntiScp207,
                            EffectType.Invigorated,
                            EffectType.DamageReduction,
                        };

                            ev.Player.AddEffect
                            (
                                effects.GetRandomValue(),
                                (byte)UnityEngine.Random.Range(1, UnityEngine.Random.Range(10, 256)),
                                UnityEngine.Random.Range(1, UnityEngine.Random.Range(10, 61)),
                                true
                            );
                        }
                        else
                        {
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
            }
        }
    }
}
