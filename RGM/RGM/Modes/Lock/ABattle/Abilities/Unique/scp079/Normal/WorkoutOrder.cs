using System.Linq;
using RGM.API.Features;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.Scp079.Common;

[Ability("운동 명령", "SCP들에게 [일반] 운동 능력을 지급합니다.", AbilityCategory.Common, AbilityType.NORMAL_SCP079_WORKOUTORDER, RoleAbility.Scp079)]

public class WorkoutOrder : Ability
{
    public override void OnEnabled()
    {
        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole() && x.Role.Type != RoleTypeId.Scp079))
        {
            scp.AddAbility(AbilityType.NORMAL_WORKOUT);
        }
    }
}