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

[Ability("인생의 동반자", "SCP-127과 섬광탄을 얻습니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP127)]
public class Scp127 : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.GrenadeFlash);
        Owner.AddItem(ItemType.GunSCP127);
    }

    public override void OnDisabled()
    {
    }
}
