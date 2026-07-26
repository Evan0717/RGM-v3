using UnityEngine;

namespace RGM.Modes.Abilities.Unique.NTF;

[Ability("산업재해보상보험", "보험 능력을 4개 얻습니다.\n20% 확률로 구사일생 능력을 2개 획득하며, 4$ 확률로 리인카네이션 능력을 획득합니다.", AbilityCategory.Common, AbilityType.NORMAL_NTF_INDUSTRIALACCIDENTINSURANCE, RoleAbility.NTF)]
public class IndustrialAccidentInsurance : Ability
{
    public override void OnEnabled()
    {
        var rand = Random.Range(1, 101);

        switch (rand)
        {
            case <= 4:
                Owner.AddAbility(AbilityType.LEGEND_REINCARNATION);
                break;
            case <= 20:
            {
                for (int i = 0; i < 3; i++) {
                    Owner.AddAbility(AbilityType.EPIC_SURVIVOR);
                }

                break;
            }
            default:
            {
                for (int i = 1; i < 5; i++)
                    Owner.AddAbility(AbilityType.NORMAL_INSURANCE);
                break;
            }
        }
    }

    public override void OnDisabled()
    {
    }
}
