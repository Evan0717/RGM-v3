using PlayerRoles;
using RGM.API.Features;
using RGM.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.RGM.Modes.Abilities.Unique.Scp079.Rare;

[Ability("은폐", "SCP들에게 [<color=#2ECCFA>희귀</color>] 투명망토 능력을 지급합니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_HIDE, RoleAbility.Scp079)]

public class Hide : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
            scp.AddAbility(AbilityType.RARE_TRANSPARENTCLOAK);
        }
    }

    public override void OnDisabled()
    {
    }
}
