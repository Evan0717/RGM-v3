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

[Ability("스테로이드", "25초 간 이동 속도가 많이 증가합니다.", AbilityCategory.Rare, AbilityType.RARE_STEROID)]
public class Steroid : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.MovementBoost, 50, 25);
    }

    public override void OnDisabled()
    {
    }
}
