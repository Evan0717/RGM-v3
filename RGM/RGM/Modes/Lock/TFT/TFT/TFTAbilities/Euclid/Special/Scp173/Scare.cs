using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp173;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

namespace DAONTFT.Core.TFT.Euclid.Scp173;

[TFTAbility("괴이", "순간이동할 때마다 해당 방이 1.5초 동안 정전됩니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp173, TFTAbilityPoint.Continuous, TFTAbilityType.Scare, "💡")]
public class Scare : TFTAbility
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;
    }

    void OnBlinking(BlinkingEventArgs ev)
    {
        ev.Player.CurrentRoom.Blackout(1.5f, DoorLockType.AdminCommand);
    }
}
