using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Epic;

[Ability("대학수학능력시험", """
                     20% 확률로 영웅(15% 확률로 전설) 능력을 3개 더 얻습니다.
                     추가 모드 [잔칫상] 활성화 시, 확률이 추가로 8%p 증가합니다.
                     """,
    AbilityCategory.Epic, AbilityType.EPIC_CSAT, RoleAbility.None, true)]
public class CSAT : Ability
{
    public override void OnEnabled()
    {
        Owner.AddHint("대학수학능력시험", "과연 결과는..?");

        Timing.CallDelayed(3.1f, () =>
        {
            if (!Owner.IsAlive) return;
            if (Random.Range(1, 101) <= (ABattle.CurrentExtraModes.Contains("잔칫상") ? 28 : 20))
            {
                Owner.AddHint("대학수학능력시험 1등급", "<b>능력을 3개 더 얻었습니다!</b>");

                for (int i = 0; i < 3; i++) {
                    var category = Random.Range(1, 101) <= 15 ? AbilityCategory.Legend : AbilityCategory.Epic;
                    Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, category, 1)[0]);
                }
                Owner.AddAbility(AbilityType.DUMMY_CSTCSUCCESS);
            }
            else
            {
                Owner.AddHint("대학수학능력시험 9등급", "다음 기회에..");
                Owner.AddAbility(AbilityType.DUMMY_CSTCFAIL);
            };
        });
    }
}