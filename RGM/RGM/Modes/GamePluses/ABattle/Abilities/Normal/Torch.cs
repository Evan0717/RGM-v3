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

namespace RGM.Modes.Abilities.Normal;

[Ability("횃불", "랜턴과 노란 사탕이 포함된 SCP-330을 지급받습니다.", AbilityCategory.Common, AbilityType.NORMAL_TORCH)]
public class Torch : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.Lantern);

        Scp330 YellowCandy = (Scp330)Item.Create(ItemType.SCP330);
        YellowCandy.RemoveAllCandy();
        YellowCandy.AddCandy(CandyKindID.Yellow);
        Owner.AddItem(YellowCandy);

        if (Owner.IsScp)
            Server.ExecuteCommand($"/forceeq {Owner.Id} 42");
    }

    public override void OnDisabled()
    {
    }
}
