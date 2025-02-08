using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Features;
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

[Ability("셧다운제", "정전 시, 해당 방의 문들은 각각 50% 확률로 닫히고 잠기게 됩니다.", AbilityCategory.Scp079, AbilityType.SCP079_SHUTDOWN)]
public class Shutdown : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.RoomBlackout += OnRoomBlackout;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.RoomBlackout -= OnRoomBlackout;
    }

    public void OnRoomBlackout(RoomBlackoutEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        foreach (var door in ev.Room.Doors)
        {
            if (Random.Range(1, 3) == 1)
            {
                door.IsOpen = false;
                door.Lock(DoorLockType.Lockdown079);

                Timing.CallDelayed(5f, () =>
                {
                    door.Unlock();
                });
            }
        }
    }
}
