using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Epic;

[Ability("대학수학능력시험", "20% 확률로 영웅(15% 확률로 전설) 능력을 3개 더 얻습니다.", AbilityCategory.Epic, AbilityType.EPIC_CSTC)]
public class CSTC : Ability
{
    public override void OnEnabled()
    {
        Owner.AddHint("대학수학능력시험", "과연 결과는..?");

        Timing.CallDelayed(3.5f, () =>
        {
            if (Owner.IsAlive)
            {
                if (Random.Range(1, 101) <= 17)
                {
                    Owner.AddHint("대학수학능력시험 1등급", "<b>능력을 3개 더 얻었습니다!</b>");

                    for (int i = 0; i < 3; i++) {
                        var category = Random.Range(1, 101) <= 17 ? AbilityCategory.Legend : AbilityCategory.Epic;
                        Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, category, 1)[0]);
                    }
                    Owner.AddAbility(AbilityType.DUMMY_CSTCSUCCESS);
                }
                else
                {
                    Owner.AddHint("대학수학능력시험 9등급", "다음 기회에..");
                    Owner.AddAbility(AbilityType.DUMMY_CSTCFAIL);
                };
            }
        });
    }

    public override void OnDisabled()
    {
    }
}