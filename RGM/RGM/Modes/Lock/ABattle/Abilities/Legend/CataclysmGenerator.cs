using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using MEC;

namespace RGM.Modes.Abilities.Legend;

[Ability("대격변 생성기", $"가지고 있는 모든 능력의 등급이 1단계 높은 등급의 랜덤한 능력으로 변경됩니다. (최고 등급: <color=#DF0101>신화</color>)", AbilityCategory.Legend, AbilityType.LEGEND_CATACLYSMGENERATOR, RoleAbility.None, true)]
public class CataclysmGenerator : Ability
{
    public override void OnEnabled()
    {
        Timing.RunCoroutine(Do());
    }
    private IEnumerator<float> Do() 
    {
        Dictionary<AbilityCategory, AbilityCategory> upgradable = new Dictionary<AbilityCategory, AbilityCategory>
        {
            { AbilityCategory.Normal, AbilityCategory.Rare },
            { AbilityCategory.Rare, AbilityCategory.Epic },
            { AbilityCategory.Epic, AbilityCategory.Legend },
            { AbilityCategory.Legend, AbilityCategory.Mythic }
        };
        List<AbilityCategory> abilityList = new List<AbilityCategory>();

        foreach (var a in Owner.GetAbilities().ToList())
        {
            if (!upgradable.ContainsKey(a.Data.Category) ||
                a.Data.AbilityType == AbilityType.LEGEND_CATACLYSMGENERATOR) continue;
            abilityList.Add(upgradable[a.Data.Category]);

            Owner.RemoveAbility(a);
            yield return Timing.WaitForOneFrame;
        }

        foreach (var ac in abilityList)
        {
            Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, ac, 3).Where(x => x != AbilityType.LEGEND_CATACLYSMGENERATOR && x != AbilityType.LEGEND_REPLICATION).GetRandomValue());
            yield return Timing.WaitForOneFrame;
        }

        yield break;
    }
}
