using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp049;

[Ability("사자", "공격 쿨타임이 50% 줄어듭니다.", AbilityCategory.Scp049, AbilityType.SCP049_DEATH)]
public class Death : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp049.Attacking += OnScp049Attacking;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp049.Attacking -= OnScp049Attacking;
    }

    public IEnumerator<float> OnScp049Attacking(Exiled.Events.EventArgs.Scp049.AttackingEventArgs ev)
    {
        if (ev.Player != Owner)
            yield break;

        yield return Timing.WaitForOneFrame;

        ev.Scp049.RemainingAttackCooldown /= 2;
    }
}
