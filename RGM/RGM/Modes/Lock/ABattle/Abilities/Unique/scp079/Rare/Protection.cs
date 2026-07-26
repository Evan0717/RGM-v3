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

[Ability("보호", "SCP들에게 [<color=#2ECCFA>희귀</color>] 강첩껍질, [일반] 바디백 능력을 지급합니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_PROTECTION, RoleAbility.Scp079)]
public class Protection : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
                scp.AddAbility(AbilityType.RARE_STEELSHELL);
                scp.AddAbility(AbilityType.NORMAL_BODYBACK);
        }
    }

    public override void OnDisabled()
    {
    }
}
