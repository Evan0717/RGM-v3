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

[Ability("무지개", "무지개 사탕이 포함된 SCP-330을 지급받습니다.", AbilityCategory.Common, AbilityType.NORMAL_RAINBOW)]
public class Rainbow : Ability
{
    public override void OnEnabled()
    {
        Owner.AddCandy(CandyKindID.Rainbow);
    }

    public override void OnDisabled()
    {
    }
}
