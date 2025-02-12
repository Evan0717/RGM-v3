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

[RequiresAbility(AbilityType.RARE_VAMPIRE, AbilityType.RARE_TRANSPARENTCLOAK)]
[Ability("뱀파이어", "<흡혈귀, 투명 망토> 마침 밤이군요, 활동할 시간입니다. 피해를 입히면 피해량의 6%를 즉시 회복합니다. <b>이 능력은 최대 체력을 무시합니다.</b>", AbilityCategory.Synergy, AbilityType.SYNERGY_VAMPIRE)]
public class Vampire : Ability
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
        if (ev.Attacker == null || ev.Attacker != Owner || !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        ev.Player.Health += (6 * ev.DamageHandler.Damage / 100);
    }
}
