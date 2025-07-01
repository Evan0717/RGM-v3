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

[Ability("요리 연구가", "'[일반] 수상한 스튜' 능력에서 긍정적인 효과만 지급됩니다.", AbilityCategory.Epic, AbilityType.EPIC_FOODRESEARCHER)]
public class FoodResearcher : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}
