using PlayerRoles;
using RGM.API.Features;
using System.Linq;

namespace RGM.Modes.Abilities.Unique.Scp079.Epic;

//[Ability("희생 명령", "SCP들에게 [<color=#FF00FF>영웅</color>]테러리스트의 유품 능력을 4개 지급합니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP079_SUICIDEORDER, RoleAbility.Scp079)]
public class SuicideOrder : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
            for (int i = 0; i < 4; i++)
            {
                scp.AddAbility(AbilityType.EPIC_TERRORISTREMAINS);
            }
        }
    }
}