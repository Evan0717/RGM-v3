using System;
using MEC;
using MultiBroadcast.Commands.Subcommands;
using RGM.API.Features;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace RGM.Modes.Abilities.Normal;

[Ability("시험", """
               35% 확률로 일반(30% 확률로 희귀) 능력을 3개 더 얻습니다.
               추가 모드 [잔칫상] 활성화 시, 확률이 추가로 15%p 증가합니다.
               """, AbilityCategory.Normal, AbilityType.NORMAL_TEST, RoleAbility.None, true)]
public class Test : Ability
{
    public static List<AbilityType> ExceptAbilities =
        [
            AbilityType.NORMAL_TEST,
            AbilityType.RARE_FINALEXAM,
            AbilityType.EPIC_CSAT,
            AbilityType.RARE_DND,
            AbilityType.RARE_TELEPORTATION
        ];
    public override void OnEnabled()
    {
        Owner.AddHint("시험", "과연 결과는..?");

        Timing.CallDelayed(3.1f, () =>
        {
            if (!Owner.IsAlive) return;
            if (Convert.ToByte(Random.Range(1, 101)) <= Mathf.Min(80,
                    4 * Owner.AbilityCount(AbilityType.NORMAL_STUDY) +
                    (ABattle.CurrentExtraModes.Contains("잔칫상") ? 50 : 35)))
            {
                Owner.AddHint("시험 성공", "<b>능력을 3개 더 얻었습니다!</b>");

                for (int i = 0; i < (Owner.HasAbility(AbilityType.SYNERGY_BRILLIANTMIND) ? 6 : 3); i++) {
                    var category = Convert.ToByte(Random.Range(1, 101)) <= 30
                        ? Convert.ToByte(Random.Range(1, 101)) <= 55 && Owner.HasAbility(AbilityType.SYNERGY_BRILLIANTMIND)
                            ? AbilityCategory.Epic : AbilityCategory.Rare : AbilityCategory.Normal;

                    Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, category, 1, ExceptAbilities)[0]);   //시험류 안 나오게 해야할 것 같음. (보류)
                }
                Owner.AddAbility(AbilityType.DUMMY_TESTSUCCESS);
            }
            else
            {
                Owner.AddHint("시험 실패", "다음 기회에..");
                Owner.AddAbility(AbilityType.DUMMY_TESTFAILURE);
            };
        });
    }
}
