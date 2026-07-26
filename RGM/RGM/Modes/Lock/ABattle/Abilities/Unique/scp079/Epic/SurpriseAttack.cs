using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Scp079;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using System;
using ProjectMER.Features.Objects;
using ProjectMER.Features;

namespace RGM.Modes.Abilities.Unique.Scp079.EPIC;


[Ability("기습", "[일반] 카메라 플래시, [<color=#2ECCFA>희귀</color>] 폭격 능력의 전조증상(빛)이 제거됩니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP079_SURPRISEATTACK, RoleAbility.Scp079)]
public class SurpriseAttack : Ability
{
    public override void OnEnabled()
    {

    }

    public override void OnDisabled()
    {

    }
}