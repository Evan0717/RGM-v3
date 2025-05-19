using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using ProjectMER.Features;
using MEC;
using RGM.API.Features;
using UnityEngine;
using RGM.API.DataBases;
using Exiled.API.Features.DamageHandlers;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.NORMAL_SWIFT, AbilityType.NORMAL_SNEAK, AbilityType.RARE_TRANSPARENTCLOAK)]
[Ability("암살자", "<경공, 잠행, 투명 망토> 더 빠르게 상대를 처단하세요. 경공 능력을 2개 더 받습니다.", AbilityCategory.Synergy, AbilityType.SYNERGY_ASSASSIN)]
public class Assassin : Ability
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            for (int i = 0; i < 2; i++)
                Owner.AddAbility(AbilityType.NORMAL_SWIFT);
        });
    }

    public override void OnDisabled()
    {
    }
}
