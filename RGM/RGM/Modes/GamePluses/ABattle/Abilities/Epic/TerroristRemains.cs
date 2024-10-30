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

[Ability("테러리스트의 유품", "핑크 사탕이 포함된 SCP-330을 지급받습니다.", AbilityCategory.Epic, AbilityType.EPIC_TERRORISTREMAINS)]
public class TerroristRemains : Ability
{
    public override void OnEnabled()
    {
        Scp330 PinkCandy = (Scp330)Item.Create(ItemType.SCP330);
        PinkCandy.RemoveAllCandy();
        PinkCandy.AddCandy(CandyKindID.Pink);
        Owner.AddItem(PinkCandy);

        if (Owner.IsScp)
            Server.ExecuteCommand($"/forceeq {Owner.Id} 42");
    }

    public override void OnDisabled()
    {
    }
}
