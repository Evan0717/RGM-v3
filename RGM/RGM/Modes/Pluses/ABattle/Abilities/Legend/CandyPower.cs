using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Legend;

[Ability("섬뜩한 힘", $"<b><color=#FF9500>[</color><color=#FF9F09>H</color><color=#FFA912>A</color><color=#FFB31B>L</color><color=#FFBD24>L</color><color=#FFC72E>O</color><color=#FFDC37>W</color><color=#FFF240>E</color><color=#FFFF49>E</color><color=#FFFF52>N</color><color=#FFFF5C>]</color></b> 모든 SCP-330의 효과가 일괄적으로 적용됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_CANDYPOWER)]
public class CandyPower : Ability
{
    public override void OnEnabled()
    {
        var effects = new List<EffectType>
        {
            EffectType.SugarRush,
            EffectType.OrangeCandy,
            EffectType.WhiteCandy,
            EffectType.Metal
        };

        foreach (var effect in effects)
        {
            Owner.AddEffect(effect, 255);
        }
    }

    public override void OnDisabled() 
    {
    }
}
