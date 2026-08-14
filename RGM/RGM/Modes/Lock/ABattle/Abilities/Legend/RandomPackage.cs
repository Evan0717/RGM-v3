using System;
using System.Collections.Generic;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using UnityEngine;

namespace RGM.Modes.Abilities.Legend;

[Ability("랜덤택배", "서버 인원 수 x2 만큼 고가치 아이템을 드롭합니다.", AbilityCategory.Legend, AbilityType.LEGEND_RANDOMPACKAGE)]
public class RandomPackage : Ability
{
    private readonly List<ItemType> _highvalueitems =
    [
        ItemType.ParticleDisruptor,
        ItemType.Jailbird,
        ItemType.MicroHID,
        ItemType.SCP1509,
        ItemType.SCP207,
        ItemType.AntiSCP207,
        ItemType.SCP268,
        ItemType.SCP500,
        ItemType.KeycardO5,
        ItemType.SCP1344,
        ItemType.GunLogicer,
        ItemType.GunFRMG0,
        ItemType.GunE11SR,
        ItemType.GunAK,
        ItemType.ArmorHeavy
    ];
    
    public override void OnEnabled()
    {
        for (int i = 1; i < Server.PlayerCount * 2; i++)
        {
            try
            {
                Item Item = Item.Create(_highvalueitems.GetRandomValue());

                Item.CreatePickup(new Vector3(Owner.Position.x, Owner.Position.y + 2, Owner.Position.z));
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to create item: {ex}");
            }
        }
    }
}
