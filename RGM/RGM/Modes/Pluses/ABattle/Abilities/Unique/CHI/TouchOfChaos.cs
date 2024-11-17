using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.CHI;

[Ability("혼돈의 손길", "지급된 동전을 튕기면 보유한 능력을 전부 삭제합니다.", AbilityCategory.CHI, AbilityType.CHI_TOUCHOFCHAOS)]
public class TouchOfChaos : Ability
{
    ushort ChaosCoinSerial;

    public override void OnEnabled()
    {
        Item Ch = Owner.AddItem(ItemType.Coin);
        ChaosCoinSerial = Ch.Serial;

        Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        ev.Item.Destroy();

        ev.Player.DisableAllEffects();

        ABattle.Instance.PlayerWorkstations[ev.Player].Clear();
        ABattle.Instance.PlayerAbilities[ev.Player].Clear();
    }
}
