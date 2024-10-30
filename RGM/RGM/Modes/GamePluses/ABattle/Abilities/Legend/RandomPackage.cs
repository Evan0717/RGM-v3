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

namespace RGM.Modes.Abilities.Legend;

[Ability("랜덤택배", "<b>서버 인원 수 x 2</b> 만큼 랜덤한 아이템을 드롭합니다.", AbilityCategory.Legend, AbilityType.LEGEND_RANDOMPACKAGE)]
public class RandomPackage : Ability
{
    public override void OnEnabled()
    {
        List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();

        for (int i = 1; i < Server.PlayerCount * 2; i++)
        {
            Item Item = Item.Create(Tools.GetRandomValue(ItemTypes));

            Item.CreatePickup(new Vector3(Owner.Position.x, Owner.Position.y + 2, Owner.Position.z));
        }
    }

    public override void OnDisabled()
    {
    }
}
