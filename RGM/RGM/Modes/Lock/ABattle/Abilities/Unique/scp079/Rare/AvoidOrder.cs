using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.Modes.Abilities.Unique.Scp079.Rare;

[Ability("회피 명령", "SCP들에게 [일반] 민첩 능력을 2개 지급합니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_AVOIDORDER, RoleAbility.Scp079)]
public class AvoidOrder : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
            for (int i = 0; i < 2; i++)
            {
                scp.AddAbility(AbilityType.NORMAL_AGILITY);
            }
        }
    }

    public override void OnDisabled()
    {
    }
}