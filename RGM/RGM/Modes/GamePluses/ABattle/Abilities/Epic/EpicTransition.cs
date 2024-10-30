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

[Ability("변이", "다음 능력 선택창에서 <color=#ffd700>전설</color> 능력 등장 확률이 25%로 조정됩니다.", AbilityCategory.Epic, AbilityType.EPIC_TRANSITION)]
public class EpicTransition : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}
