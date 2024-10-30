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

[Ability("잠수부", "시야가 개선되고 스테미나가 줄어들지 않습니다.", AbilityCategory.Epic, AbilityType.EPIC_DIVER)]
public class Diver : Ability
{
    public override void OnEnabled()
    {
        Owner.GetEffect(EffectType.Invigorated).Intensity += 1;
    }

    public override void OnDisabled()
    {
        Owner.GetEffect(EffectType.Invigorated).Intensity -= 1;
    }
}
