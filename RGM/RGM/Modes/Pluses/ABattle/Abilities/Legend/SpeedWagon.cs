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

[Ability("스피드왜건", "속도가 크게 증가합니다.", AbilityCategory.Legend, AbilityType.LEGEND_SPEEDWAGON)]
public class SpeedWagon : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.MovementBoost, 100);
    }

    public override void OnDisabled()
    {
    }
}
