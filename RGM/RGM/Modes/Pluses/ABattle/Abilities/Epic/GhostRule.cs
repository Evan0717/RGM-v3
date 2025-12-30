using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using ProjectMER.Features.Objects;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Epic;

[Ability("고스트룰", "유령이 되어 문을 통과할 수 있게 됩니다.", AbilityCategory.Epic, AbilityType.EPIC_GHOSTRULE)]
public class GhostRule : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.Ghostly, 1);
    }

    public override void OnDisabled()
    {
        Owner.RemoveEffect(EffectType.Ghostly, 1);
    }
}
