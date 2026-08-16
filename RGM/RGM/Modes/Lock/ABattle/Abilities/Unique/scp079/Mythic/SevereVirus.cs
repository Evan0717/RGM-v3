using RGM.API.Features;
using System.Linq;
using UnityEngine;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.Scp079.Mythic;


[Ability("치명적인 바이러스", "아군들에게 [<color=#ffd700>전설</color>] 상급 변이 능력을 3 ~ 4개와 [<color=#FF00FF>영웅</color>] 변이 능력을 10 ~ 12개 지급합니다.\n자신은 [<color=#FF00FF>영웅</color>]변이 능력을 2, 3개 얻습니다.", AbilityCategory.Mythic, AbilityType.MYTHIC_SCP079_SEVEREVIRUS, RoleAbility.Scp079)]
public class SevereVirus : Ability
{
    public override void OnEnabled()
    {
        foreach (var p in PlayerManager.List.Where(x => x.LeadingTeam == Owner.LeadingTeam && x.IsAlive))
        {
            if (p == Owner)
            {
                for (int i = 0; i < Random.Range(2, 4); i++)
                {
                    p.AddAbility(AbilityType.EPIC_TRANSITION);
                }
                continue;
            }

            for (int i = 0; i < Random.Range(3, 5); i++)
            {
                p.AddAbility(AbilityType.LEGEND_TRANSITION);
            }

            for (int i = 0; i < Random.Range(10, 13); i++)
            {
                p.AddAbility(AbilityType.EPIC_TRANSITION);
            }
        }
    }
}