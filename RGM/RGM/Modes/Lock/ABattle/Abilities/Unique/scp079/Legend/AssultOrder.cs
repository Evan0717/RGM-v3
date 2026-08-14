using System.Linq;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp079.Legend;

[Ability("돌격 명령", "SCP들에게 [<color=#FF00FF>영웅</color>]람보, [<color=#FF00FF>영웅</color>]샤프 아이즈, [<color=#FF00FF>영웅</color>]몰락한 왕의 검 능력을 지급합니다.", AbilityCategory.Legend, AbilityType.LEGEND_SCP079_ASSULTORDER, RoleAbility.Scp079)]

public class AssultOrder : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079)) {
            scp.AddAbility(AbilityType.EPIC_RAMBO);
            scp.AddAbility(AbilityType.EPIC_FALLENKINGSSWORD);
            scp.AddAbility(AbilityType.EPIC_SHARPEYES);
        }
    }
}