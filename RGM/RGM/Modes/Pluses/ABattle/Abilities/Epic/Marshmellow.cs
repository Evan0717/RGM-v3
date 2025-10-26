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

[Ability("!!마쉬멜로우!!", "<b><color=#FF9500>[</color><color=#FF9F09>H</color><color=#FFA912>A</color><color=#FFB31B>L</color><color=#FFBD24>L</color><color=#FFC72E>O</color><color=#FFDC37>W</color><color=#FFF240>E</color><color=#FFFF49>E</color><color=#FFFF52>N</color><color=#FFFF5C>]</color></b> 즉시 마쉬멜로우맨이 됩니다. 체력은 500이며, \"경공\" 능력을 2개 얻습니다.", AbilityCategory.Epic, AbilityType.EPIC_MARSHMELLOW)]
public class MarshMellow : Ability
{
    public override void OnEnabled()
    {
        Owner.MaxHealth = 500;
        Owner.Health = Owner.MaxHealth;
        Owner.AddAbility(AbilityType.NORMAL_SWIFT);
        Owner.AddAbility(AbilityType.NORMAL_SWIFT);
    }

    public override void OnDisabled()
    {
    }
}
