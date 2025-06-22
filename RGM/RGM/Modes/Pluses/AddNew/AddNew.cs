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

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.AddNew)]
    class AddNew : Mode
    {
        public override string Name => "요소 추가";
        public override string Description => "시설에 무언가가 더 추가됩니다.";
        public override string Detail =>
"""
- 자판기
""";
        public override string Color => "c4ee06";

        public static AdditionalWave Instance;

        static Dictionary<ItemType, float> vandingMachineChances { get; set; } = new()
        {
            { ItemType.SCP330, 85 },
            { ItemType.SCP207, 5 },
            { ItemType.Jailbird, 5 },
            { ItemType.AntiSCP207, 4 },
            { ItemType.MicroHID, 1 }
        };

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;

            Timing.RunCoroutine(OnModeStarted());
        }

        public static IEnumerator<float> OnModeStarted()
        {
            Tools.LoadMap("vm");

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
                else
                {
                    Tools.PlaySound(sound, "vm_insert_fail", 2);
                }
            }
        }
    }
}
