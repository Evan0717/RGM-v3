using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp106;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp939;

[Ability("민첩한 사냥 도구", "공격 쿨타임이 25% 줄어듭니다.", AbilityCategory.Scp939, AbilityType.SCP939_AGILEHUNTINGTOOL)]
public class AgileHuntingTool : Ability
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp939Role Scp939)
            Scp939.AttackCooldown /= 4;
    }

    public override void OnDisabled()
    {
    }
}
