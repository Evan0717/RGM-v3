using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("기말고사", "33% 확률로 희귀(27% 확률로 영웅) 능력을 3개 더 얻습니다.", AbilityCategory.Rare, AbilityType.RARE_FINALEXAM)]
public class FinalExam : Ability
{
    public override void OnEnabled()
    {
        Owner.AddHint("기말고사", "과연 결과는..?");

        Timing.CallDelayed(3.5f, () =>
        {
            if (Owner.IsAlive)
            {
                if (Random.Range(1, 101) <= 33)
                {
                    Owner.AddHint("기말고사 수석", "<b>능력을 3개 더 얻었습니다!</b>");

                    for (int i = 0; i < 3; i++) {
                        var category = Random.Range(1, 101) <= 27 ? AbilityCategory.Epic : AbilityCategory.Rare;
                        Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, category, 1)[0]);
                    }
                    Owner.AddAbility(AbilityType.DUMMY_FINALEXAMSUCCESS);
                }
                else
                {
                    Owner.AddHint("기말고사 낙제", "다음 기회에..");
                    Owner.AddAbility(AbilityType.DUMMY_FINALEXAMFAIL);
                };
            }
        });
    }

    public override void OnDisabled()
    {
    }
}