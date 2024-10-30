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

[Ability("상급 변이", "다음 능력 선택창에서 <color=#DF0101>신화</color> 능력 등장 확률이 25%로 조정됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_TRANSITION)]
public class LEGENDTransition : Ability
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}
