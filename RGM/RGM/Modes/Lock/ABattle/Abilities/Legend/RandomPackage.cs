using System;
using System.Collections.Generic;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using UnityEngine;

namespace RGM.Modes.Abilities.Legend;

//[Ability("랜덤택배", "고가치 아이템을 30개 드롭합니다.", AbilityCategory.Legend, AbilityType.LEGEND_RANDOMPACKAGE)]
public class RandomPackage : Ability
{
    private readonly List<ItemType> _highvalueitems =
    [
        ItemType.ParticleDisruptor,
        ItemType.Jailbird,
        ItemType.MicroHID,
        ItemType.SCP1509,
        ItemType.SCP1853,
        ItemType.AntiSCP207,
        ItemType.SCP268,
        ItemType.SCP500,
        ItemType.KeycardO5,
        ItemType.SCP1344,
        ItemType.GunLogicer,
        ItemType.GunE11SR,
        ItemType.ArmorHeavy
    ];
    
    public override void OnEnabled()
    {
        for (int i = 1; i < 31; i++)
        {
            try
            {
                Item item = Item.Create(_highvalueitems.GetRandomValue());

                if (item is Firearm firearm)
                    firearm.MagazineAmmo = firearm.MaxMagazineAmmo;

                item.CreatePickup(new Vector3(Owner.Position.x, Owner.Position.y + 2, Owner.Position.z));
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to create item: {ex}");
            }
        }
    }
}
