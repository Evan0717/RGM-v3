using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp096;

[Ability("원수", "분노 충전 시간이 50% 줄어듭니다.", AbilityCategory.Scp096, AbilityType.SCP096_ENEMY)]
public class Enemy : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp096.Charging += OnCharging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp096.Charging -= OnCharging;
    }

    public void OnCharging(Exiled.Events.EventArgs.Scp096.ChargingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            ev.Scp096.RemainingChargeDuration /= 2;
        });
    }
}
