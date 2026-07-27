using System.Linq;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("보건소 직원", "모든 아군들에게 SCP-500을 지급합니다.", AbilityCategory.Common, AbilityType.NORMAL_NTF_HEALTHCENTERSTAFF, RoleAbility.NTF)]
public class HealthCenterStaff : Ability
{
    public override void OnEnabled()
    {
        foreach (var team in PlayerManager.List.Where(x => 
                     !x.IsNPC && 
                     x.IsAlive && 
                     !HitboxIdentity.IsEnemy(Owner.ReferenceHub, x.ReferenceHub)))
            team.AddItem(ItemType.SCP500);
    }

    public override void OnDisabled()
    {
    }
}
