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

[Ability("무기 전문가", "SCP-1853을 받습니다.", AbilityCategory.Rare, AbilityType.RARE_WEAPONEXPERT)]
public class WeaponExpert : Ability
{
    public override void OnEnabled()
    {
        Item scp1853 = Owner.AddItem(ItemType.SCP1853);

        if (Owner.IsScp)
            Owner.CurrentItem = scp1853;
    }

    public override void OnDisabled()
    {
    }
}
