using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.Patches.Events.Player;
using MEC;
using ProjectMER.Features.Serializable;
using RGM.API.Features;
using System.Linq;
using UnityEngine;
using PlayerRoles;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Unique.Scp079.Mythic;


[Ability("치명적인 바이러스", "SCP들에게 [<color=#ffd700>전설</color>] 상급 변이 능력을 2 ~ 3개 지급합니다.", AbilityCategory.Mythic, AbilityType.MYTHIC_SCP079_SEVEREVIRUS, RoleAbility.Scp079)]
public class SevereVirus : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
            for (int i = 0; i < Random.Range(2, 4); i++)
            {
                scp.AddAbility(AbilityType.LEGEND_TRANSITION);
            }
        }
    }

    public override void OnDisabled()
    {

    }
}