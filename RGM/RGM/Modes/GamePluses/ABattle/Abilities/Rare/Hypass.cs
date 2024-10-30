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

[Ability("하이패스", "25초 간 무적이 됩니다.", AbilityCategory.Rare, AbilityType.RARE_HYPASS)]
public class Hypass : Ability
{
    public override void OnEnabled()
    {
        Owner.IsGodModeEnabled = true;

        Timing.CallDelayed(25, () => { Owner.IsGodModeEnabled = false; });
    }

    public override void OnDisabled()
    {
    }
}
