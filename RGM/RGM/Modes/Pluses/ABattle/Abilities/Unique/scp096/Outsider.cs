using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Unique.Scp096;

[Ability("아웃사이더", "몸이 20% 투명해집니다.", AbilityCategory.Scp096, AbilityType.SCP096_OUTSIDER)]
public class Outsider : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.Fade, 20);
    }

    public override void OnDisabled()
    {
    }
}
