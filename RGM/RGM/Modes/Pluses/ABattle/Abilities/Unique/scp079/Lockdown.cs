using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Scp106;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("봉쇄", "10초 간 모든 문을 닫고, 잠급니다. 모든 방이 정전됩니다.", AbilityCategory.Scp079, AbilityType.SCP079_LOCKDOWN)]
public class Lockdown : Ability
{
    public override void OnEnabled()
    {
        foreach (var door in Door.List.Where(x => !x.IsElevator))
        {
            door.IsOpen = false;
            door.Lock(1205, DoorLockType.Lockdown079);
        }

        Timing.CallDelayed(10, () =>
        {
            foreach (var door in Door.List)
                door.Unlock();
        });

        Map.TurnOffAllLights(10);
    }

    public override void OnDisabled()
    {

    }
}
