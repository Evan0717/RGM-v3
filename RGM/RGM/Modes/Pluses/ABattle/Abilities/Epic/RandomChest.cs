using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Epic;

[Ability("랜덤상자", "랜덤하지만 좋은 아이템을 2개 지급받습니다.", AbilityCategory.Epic, AbilityType.EPIC_RANDOMCHEST)]
public class RandomChest : Ability
{
    public override void OnEnabled()
    {
        List<ItemType> RandomChest = new List<ItemType>()
        {
            ItemType.ParticleDisruptor,
            ItemType.Jailbird,
            ItemType.MicroHID,
            ItemType.SCP018,
            ItemType.SCP1576,
            ItemType.SCP2176,
            ItemType.SCP207,
            ItemType.AntiSCP207,
            ItemType.SCP268,
            ItemType.SCP500,
            ItemType.KeycardO5,
            ItemType.SCP1344
        };

        for (int i = 1; i < 3; i++)
        {
            Item RandomChestItem = Owner.AddItem(Tools.GetRandomValue(RandomChest));
        }
    }

    public override void OnDisabled()
    {
    }
}
