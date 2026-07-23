using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using RGM.API.Features;
using PlayerRoles;


namespace RGM.Modes.Abilities.Unique.Scp079.Epic;

[Ability("공격 명령", "SCP들에게 [일반] 단련 능력을 2개 지급합니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP079_ATTACKORDER, RoleAbility.Scp079)]
public class AttackOrder : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
            for (int i = 0; i < 2; i++)
            {
                scp.AddAbility(AbilityType.NORMAL_TRAINING);
            }
        }
    }
    public override void OnDisabled()
    {
    }
}

