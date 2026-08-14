using PlayerRoles;
using RGM.API.Features;
using System.Linq;

namespace RGM.Modes.Abilities.Unique.Scp079.Rare;

[Ability("보호", "SCP들에게 [<color=#2ECCFA>희귀</color>] 하이패스, [<color=#2ECCFA>희귀</color>] 강첩껍질 능력 3개 지급합니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP079_PROTECTION, RoleAbility.Scp079)]
public class Protection : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
            for (int i = 0; i < 3; i++)
            {
                scp.AddAbility(AbilityType.RARE_STEELSHELL);
            }
            scp.AddAbility(AbilityType.RARE_HYPASS);
        }
    }
}
