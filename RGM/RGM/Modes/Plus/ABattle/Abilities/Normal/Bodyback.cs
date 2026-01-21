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

[Ability("바디백", "몸통 데미지 경감 효과가 3% 추가됩니다.", AbilityCategory.Common, AbilityType.NORMAL_BODYBACK)]
public class Bodyback : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.BodyshotReduction, 6);
    }

    public override void OnDisabled()
    {
        Owner.RemoveEffect(EffectType.BodyshotReduction, 6);
    }
}
