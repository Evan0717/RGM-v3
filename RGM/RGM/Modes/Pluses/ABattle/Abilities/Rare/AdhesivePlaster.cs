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

namespace RGM.Modes.Abilities.Rare;

[Ability("반창고", "체력이 절반 이하로 줄어들었을 경우 120HP를 즉시 회복합니다. (최대 체력 무시)", AbilityCategory.Rare, AbilityType.RARE_ADHESIVEPLASTER)]
public class AdhesivePlaster : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (ev.Player.Health <= ev.Player.MaxHealth / 2)
        {
            ev.Player.Health += 120;

            ev.Player.RemoveAbility(AbilityType.RARE_ADHESIVEPLASTER);

            Owner.AddAbility(AbilityType.DUMMY_USEDADHESIVEPLASTER);
            Owner.AddHint("반창고", $"<color={ABattle.RatingColor["희귀"]}>반창고</color> 효과 덕에 체력을 120HP 회복했습니다.");
        }
    }
}
