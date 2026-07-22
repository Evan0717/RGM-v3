using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.Patches.Events.Player;
using MEC;
using ProjectMER.Features.Serializable;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Unique.Scp079.Legend;


[Ability("블랙아웃", "시설 전체가 정전됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_SCP079_BLACKOUT, RoleAbility.Scp079)]
public class Blackout: Ability
{
    public override void OnEnabled()
    {
        foreach (var room in Room.List)
        {
           room.TurnOffLights();
        }
    }

    public override void OnDisabled()
    {

    }
}