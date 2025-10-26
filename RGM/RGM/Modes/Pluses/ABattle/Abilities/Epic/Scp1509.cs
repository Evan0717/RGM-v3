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

[Ability("마체테", "SCP-1509를 지급받습니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP1509)]
public class Scp1509 : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.SCP1509);
    }

    public override void OnDisabled()
    {
    }
}
