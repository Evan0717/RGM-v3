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

[Ability("도박꾼", "아이템을 버리면 새로운 아이템을 받지만, 2% 확률로 손이 잘립니다.", AbilityCategory.Epic, AbilityType.EPIC_GAMBLER)]
public class Gambler : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.DroppedItem += OnDroppedItem;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.DroppedItem -= OnDroppedItem;
    }

    public void OnDroppedItem(DroppedItemEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (UnityEngine.Random.Range(1, 101) < 3)
            ev.Player.EnableEffect(EffectType.SeveredHands);

        else
        {
            ev.Pickup.Destroy();

            List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();

            Item Item = Item.Create(Tools.GetRandomValue(ItemTypes));
            Item.CreatePickup(ev.Player.Position);
        }
    }
}
