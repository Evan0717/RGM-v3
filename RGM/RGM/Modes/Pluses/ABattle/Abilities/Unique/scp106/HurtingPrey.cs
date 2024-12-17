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

namespace RGM.Modes.Abilities.Unique.Scp106;

[Ability("사냥감 모색", "공격 성공 후 속도가 일시적으로 증가합니다.", AbilityCategory.Scp106, AbilityType.SCP106_HUNTINGPREY)]
public class HurtingPrey : Ability
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
        if (ev.Attacker == null || ev.Attacker != Owner)
            return;

        ev.Attacker.GetEffect(EffectType.MovementBoost).Intensity += 25;

        Timing.CallDelayed(3f, () =>
        {
            if (ev.Attacker.GetEffect(EffectType.MovementBoost).Intensity >= 25)
                ev.Attacker.GetEffect(EffectType.MovementBoost).Intensity -= 25;

            else
                ev.Attacker.GetEffect(EffectType.MovementBoost).Intensity = 0;
        });
    }
}
