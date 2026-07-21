using System.Linq;
using Exiled.API.Enums;
using PlayerRoles;
using RGM.API.Features;
namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("신속 지원", "SCP들에게 경공 능력 3개를 지급합니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP079_SWIFTSUPPORT, RoleAbility.Scp079)]
public class SwiftSupport : Ability
{
    public override void OnEnabled()
    {

        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
            for (int i = 0; i < 3; i++)
            {
                scp.AddAbility(AbilityType.NORMAL_SWIFT);
            }
        }
    }


    public override void OnDisabled()
    {
    }
}
