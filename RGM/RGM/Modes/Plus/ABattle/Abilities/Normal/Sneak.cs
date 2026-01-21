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

[Ability("잠행", "발걸음 소리가 줄어듭니다.", AbilityCategory.Common, AbilityType.NORMAL_SNEAK)]
public class Sneak : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.SilentWalk, 3);
    }

    public override void OnDisabled()
    {
        Owner.RemoveEffect(EffectType.SilentWalk, 3);
    }
}
