using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Epic;

[Ability("투시", "SCP-1344 효과를 받습니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP1344)]
public class Scp1344 : Ability
{
    public override void OnEnabled()
    {
        Owner.EnableEffect(EffectType.Scp1344, 1);
    }

    public override void OnDisabled()
    {

    }
}
