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

namespace RGM.Modes.Abilities.Epic;

[Ability("극독", "죽인 자에게 20초 동안 심장 마비 효과를 부여합니다.", AbilityCategory.Epic, AbilityType.EPIC_EXTREMEPOISON)]
public class ExtremePoison : Ability
{
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
        if (ev.Player != Owner || ev.Attacker == null)
            return;

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (Owner.IsDead)
                ev.Attacker.EnableEffect(EffectType.CardiacArrest, 1, 20 * Owner.AbilityCount(AbilityType.EPIC_EXTREMEPOISON));
        });
    }
}