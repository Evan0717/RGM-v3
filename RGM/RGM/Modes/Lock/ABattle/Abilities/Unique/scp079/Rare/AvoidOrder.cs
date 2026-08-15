using PlayerRoles;
using RGM.API.Features;
using System.Linq;

namespace RGM.Modes.Abilities.Unique.Scp079.Rare;

[Ability("회피 명령", "SCP들에게 [일반] 민첩 능력을 3개 지급합니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_AVOIDORDER, RoleAbility.Scp079)]
public class AvoidOrder : Ability
{
    public override void OnEnabled()
    {
        foreach (var p in PlayerManager.List.Where(x => x.LeadingTeam == Owner.LeadingTeam && x.IsAlive && x.Role != RoleTypeId.Scp079))
        {
            for (int i = 0; i < 3; i++)
            {
                p.AddAbility(AbilityType.NORMAL_AGILITY);
            }
        }
    }
}