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

[Ability("회춘", "공격 쿨타임이 50% 줄어듭니다.", AbilityCategory.Scp106, AbilityType.SCP106_RECOVERY)]
public class Recovery : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp106.Attacking += OnAttacking;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp106.Attacking -= OnAttacking;
    }

    public void OnAttacking(AttackingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            ev.Scp106.CaptureCooldown /= 2;
        });
    }
}
