using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Synergy;

[RequiresAbility(AbilityType.DUMMY_LEGENDTRANSITIONFAILURE, AbilityType.DUMMY_EPICTRANSITIONFAILURE, AbilityType.DUMMY_RARETRANSITIONFAILURE)]
[Ability("패배자", "<하급 변이 실패, 변이 실패, 상급 변이 실패> 음.. 이건 좀 안타깝네요.", AbilityCategory.Synergy, AbilityType.SYNERGY_LOSER)]
public class Loser : Ability
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(1, () =>
        {
            Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, AbilityCategory.Legend, 1).GetRandomValue());
        });
    }

    public override void OnDisabled()
    {
    }
}
