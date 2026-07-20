using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("시험", "35% 확률로 일반(30% 확률로 희귀) 능력을 3개 더 얻습니다.", AbilityCategory.Common, AbilityType.NORMAL_TEST)]
public class Test : Ability
{
    public override void OnEnabled()
    {
        Owner.AddHint("시험", "과연 결과는..?");

        Timing.CallDelayed(3.5f, () =>
        {
            if (Owner.IsAlive)
            {
                if (Random.Range(1, 101) <= 35)
                {
                    Owner.AddHint("시험 성공", "<b>능력을 3개 더 얻었습니다!</b>");

                    for (int i = 0; i < 3; i++) {
                        var category = Random.Range(1, 101) <= 30 ? AbilityCategory.Rare : AbilityCategory.Common;
                        Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, category, 1,[AbilityType.RARE_DND, AbilityType.RARE_TELEPORTATION])[0]);
                    }
                    Owner.AddAbility(AbilityType.DUMMY_TESTSUCCESS);
                }
                else
                {
                    Owner.AddHint("시험 실패", "다음 기회에..");
                    Owner.AddAbility(AbilityType.DUMMY_TESTFAILURE);
                };
            }
        });
    }

    public override void OnDisabled()
    {
    }
}
