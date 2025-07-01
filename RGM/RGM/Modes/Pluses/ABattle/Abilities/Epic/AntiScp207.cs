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

[Ability("초재생", "안티 콜라를 지급받습니다. 운이 좋다면 2개가 지급됩니다.", AbilityCategory.Epic, AbilityType.EPIC_ANTISCP207)]
public class AntiScp207 : Ability
{
    public override void OnEnabled()
    {
        Owner.AddItem(ItemType.AntiSCP207);

        if (UnityEngine.Random.Range(1, 6) == 1)
            Owner.AddItem(ItemType.AntiSCP207);
    }

    public override void OnDisabled()
    {
    }
}
