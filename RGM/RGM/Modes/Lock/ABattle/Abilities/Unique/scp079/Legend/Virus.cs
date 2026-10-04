using RGM.API.Features;
using System.Linq;

namespace RGM.Modes.Abilities.Unique.Scp079.Legend;

[Ability("바이러스", "아군들에게 [<color=#FF00FF>영웅</color>] 랜덤 능력 2개, [<color=#FF00FF>영웅</color>] 변이 능력을 1개 지급합니다.",
    AbilityCategory.Legend, AbilityType.LEGEND_SCP079_VIRUS, RoleAbility.Scp079)]
public class Virus : Ability
{
    public override void OnEnabled()
    {
        foreach (var player in PlayerManager.List.Where(x => x.LeadingTeam == Owner.LeadingTeam && x.IsAlive))
        {
            player.AddAbility(ABattle.Instance.GetRandomAbilities(player, AbilityCategory.Epic, 2)[0]);
            player.AddAbility(AbilityType.EPIC_TRANSITION);
        }
    }
}