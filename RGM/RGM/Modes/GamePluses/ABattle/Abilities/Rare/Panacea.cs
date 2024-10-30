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

[Ability("만병통치약", "SCP-500을 받습니다.", AbilityCategory.Rare, AbilityType.RARE_PANACEA)]
public class Panacea : Ability
{
    public override void OnEnabled()
    {
        Item scp500 = Owner.AddItem(ItemType.SCP500);

        if (Owner.IsScp)
            Owner.CurrentItem = scp500;
    }

    public override void OnDisabled()
    {
    }
}
