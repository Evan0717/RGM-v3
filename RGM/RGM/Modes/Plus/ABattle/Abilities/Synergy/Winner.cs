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

[RequiresAbility(AbilityType.DUMMY_LEGENDTRANSITIONSUCCESS, AbilityType.DUMMY_EPICTRANSITIONSUCCESS, AbilityType.DUMMY_RARETRANSITIONSUCCESS)]
[Ability("승리자", "<하급 변이 성공, 변이 성공, 상급 변이 성공> 오늘은 운수가 좋군요.", AbilityCategory.Synergy, AbilityType.SYNERGY_WINNER)]
public class Winner : Ability
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(1, () =>
        {
            Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, AbilityCategory.Mythic, 2).GetRandomValue());
        });
    }

    public override void OnDisabled()
    {
    }
}
