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
        if (Owner.IsScpRole() || Owner.Role.Type.ToString().Contains("Flamingo"))
            Owner.AddHint("도박", $"<size=20>[Space + ALT]ㅣ도박을 진행할 수 있습니다.</size>", 10);

        Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
        Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
        Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
    }

    public void OnDroppingItem(DroppingItemEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        List<ItemType> ItemList = Tools.EnumToList<ItemType>();
        ItemType Item = Tools.GetRandomValue(ItemList);

        int rand = UnityEngine.Random.Range(1, 101);
        if (0 < rand && rand < 3)
        {
            Owner.EnableEffect(EffectType.SeveredHands, 1, 50);
        }
        else
        {
            ev.Item.Destroy();
            Item CurrentItem = Owner.AddItem(Item);
            Owner.DropItem(CurrentItem);
        }
    }

    public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
    {
        if (!(Owner.IsScpRole() || Owner.Role.Type.ToString().Contains("Flamingo")) || !Owner.IsJumping || Owner.GetEffect(EffectType.SeveredHands).IsEnabled)
            return;

        int rand = UnityEngine.Random.Range(1, 101);

        if (0 < rand && rand < 3)
            Owner.EnableEffect(EffectType.SeveredHands, 1, 50);

        else
        {
            List<ItemType> ItemList = Tools.EnumToList<ItemType>();
            ItemType Item = Tools.GetRandomValue(ItemList);

            Owner.AddItem(Item);
        }
    }
}
