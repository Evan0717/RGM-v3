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

[Ability("강철 껍질", "데미지 경감 효과가 5% 추가됩니다.", AbilityCategory.Rare, AbilityType.RARE_STEELSHELL)]
public class SteelShell : Ability
{
    public override void OnEnabled()
    {
        Owner.GetEffect(EffectType.DamageReduction).Intensity += 10;
    }

    public override void OnDisabled()
    {
        Owner.GetEffect(EffectType.DamageReduction).Intensity -= 10;
    }
}
