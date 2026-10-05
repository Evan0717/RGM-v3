using System;
using RGM.API.Features;
using System.Linq;
using Random = UnityEngine.Random;
    
namespace RGM.Modes.Abilities.Unique.Scp079.Mythic;

[Ability("치명적인 바이러스", "아군들에게 랜덤 <color=#ffd700>전설</color> 능력 1개를 지급하며, [<color=#ffd700>전설</color>] 상급 변이 능력을 1개 지급합니다.", 
    AbilityCategory.Mythic, AbilityType.MYTHIC_SCP079_SEVEREVIRUS, RoleAbility.Scp079)]

public class SevereVirus : Ability
{
    public override void OnEnabled()
    {
        foreach (var p in PlayerManager.List.Where(x => x.LeadingTeam == Owner.LeadingTeam && x.IsAlive && x != Owner))
        {
            p.AddAbility(AbilityType.LEGEND_TRANSITION);
            p.AddAbility(ABattle.Instance.GetRandomAbilities(p, AbilityCategory.Legend, 1)[0]);
        }
    }
}