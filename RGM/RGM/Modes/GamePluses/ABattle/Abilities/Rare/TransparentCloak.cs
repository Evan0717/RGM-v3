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

[Ability("투명 망토", "25초 간 투명 효과를 받습니다.", AbilityCategory.Rare, AbilityType.RARE_TRANSPARENTCLOAK)]
public class TransparentCloak : Ability
{
    public override void OnEnabled()
    {
        Owner.EnableEffect(EffectType.Invisible, 1, 25);
    }

    public override void OnDisabled()
    {
        Owner.DisableEffect(EffectType.Invisible);
    }
}
