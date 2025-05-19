using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
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

[Ability("도굴꾼", "사망한 아군의 능력 중 하나를 랜덤으로 획득합니다. (4회)", AbilityCategory.Epic, AbilityType.EPIC_GRAVEROBBER)]
public class GraveRobber : Ability
{
    int count = 4;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player.LeadingTeam != Owner.LeadingTeam || ev.Player.GetAbilities().Count() == 0)
            return;

        List<AbilityType> abilityTypes = ev.Player.GetAbilities().Select(x => x.Data.AbilityType).ToList();

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (ev.Player.IsDead)
            {
                Owner.AddAbility(abilityTypes.GetRandomValue());

                if (--count == 0)
                {
                    Owner.RemoveAbility(AbilityType.EPIC_GRAVEROBBER);
                    OnDisabled();
                    Owner.AddAbility(AbilityType.DUMMY_ENDOFGRAVEROBBERY);
                }
            }
        });
    }
}
