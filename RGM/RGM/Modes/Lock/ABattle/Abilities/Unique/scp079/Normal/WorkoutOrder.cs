using System.Linq;
using RGM.API.Features;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.Scp079.Common;

[Ability("운동 명령", "아군들에게 [일반] 운동 능력을 지급합니다.", AbilityCategory.Normal, AbilityType.NORMAL_SCP079_WORKOUTORDER, RoleAbility.Scp079)]

public class WorkoutOrder : Ability
{
    public override void OnEnabled()
    {
        foreach (var p in PlayerManager.List.Where(x => x.LeadingTeam == Owner.LeadingTeam && x.IsAlive && x.Role != RoleTypeId.Scp079))
        {
            p.AddAbility(AbilityType.NORMAL_WORKOUT);
        }
    }
}