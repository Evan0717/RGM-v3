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

namespace RGM.Modes.Abilities.Rare;

[Ability("계약", "지급된 동전을 튕기면 당장 죽지만, 다음 생에 능력 3개를 가진 채로 시작합니다.", AbilityCategory.Rare, AbilityType.RARE_CONTRACT)]
public class Contract : Ability
{
    public override void OnEnabled()
    {
        Item cc = player.AddItem(ItemType.Coin);
        ContractCoinSerials.Add(cc.Serial);

        if (Owner.IsScp)
            Owner.CurrentItem = cc;
    }

    public override void OnDisabled()
    {
    }
}
