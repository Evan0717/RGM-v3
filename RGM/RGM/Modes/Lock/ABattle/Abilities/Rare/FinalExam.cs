using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("기말고사", """
                 25% 확률로 희귀(20% 확률로 영웅) 능력을 3개 더 얻습니다.
                 추가 모드 [잔칫상] 활성화 시, 확률이 추가로 11%p 증가합니다.
                 """, AbilityCategory.Rare, AbilityType.RARE_FINALEXAM, RoleAbility.None, true)]
public class FinalExam : Ability
{
    public override void OnEnabled()
    {
        Owner.AddHint("기말고사", "과연 결과는..?");

        Timing.CallDelayed(3.1f, () =>
        {
            if (!Owner.IsAlive) return;
            if (Random.Range(1, 101) <= (ABattle.CurrentExtraModes.Contains("잔칫상") ? 36 : 25))
            {
                Owner.AddHint("기말고사 수석", "<b>능력을 3개 더 얻었습니다!</b>");

                for (int i = 0; i < 3; i++) {
                    var category = Random.Range(1, 101) <= 20 ? AbilityCategory.Epic : AbilityCategory.Rare;
                    Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, category, 1, [AbilityType.RARE_DND, AbilityType.RARE_TELEPORTATION])[0]);
                }
                Owner.AddAbility(AbilityType.DUMMY_FINALEXAMSUCCESS);
            }
            else
            {
                Owner.AddHint("기말고사 낙제", "다음 기회에..");
                Owner.AddAbility(AbilityType.DUMMY_FINALEXAMFAIL);
            };
        });
    }
}