using System.Linq;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Human.Rare;

[Ability("의무병", "자신과 아군들에게 [희귀] 육체 강화 1개를 지급합니다.",
    AbilityCategory.Rare, AbilityType.RARE_HUMAN_MEDICALOFFICER, RoleAbility.Human)]
public class MedicalOfficer : Ability
{

    public override void OnEnabled()
    {
        foreach (var team in PlayerManager.List.Where(x =>
                     x.LeadingTeam == Owner.LeadingTeam && 
                     x.IsAlive))
        {
            team.AddAbility(AbilityType.RARE_PHYSICALSTRENGTHENING);
        }
    }
}
