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

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility([AbilityType.NORMAL_RANDOMBOX, AbilityType.EPIC_RANDOMCHEST, AbilityType.LEGEND_RANDOMPACKAGE])]
[Ability("랜덤 컬렉션", "<랜덤박스, 랜덤상자, 랜덤택배> 3+1! 셋 중 하나를 더 받으세요!", AbilityCategory.Synergy, AbilityType.SYNERGY_RANDOMCOLLECTION)]
public class RandomCollection : Ability
{
    public override void OnEnabled()
    {
        List<AbilityType> Randoms = new List<AbilityType>()
        {
            AbilityType.NORMAL_RANDOMBOX,
            AbilityType.EPIC_RANDOMCHEST,
            AbilityType.LEGEND_RANDOMPACKAGE
        };

        Owner.AddAbility(Tools.GetRandomValue(Randoms));
    }

    public override void OnDisabled()
    {
    }
}
