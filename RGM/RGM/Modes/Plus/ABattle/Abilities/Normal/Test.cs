using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using MultiBroadcast.API;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("시험", "15% 확률로 능력을 3개 더 얻습니다.", AbilityCategory.Common, AbilityType.NORMAL_TEST)]
public class Test : Ability
{
    public override void OnEnabled()
    {
        Owner.AddHint("시험", "과연 결과는..?");

        Timing.CallDelayed(3.5f, () =>
        {
            if (Owner.IsAlive)
            {
                if (Random.Range(1, 101) <= 15)
                {
                    Owner.AddHint("시험 성공", "<b>능력을 3개 더 얻었습니다!</b>");

                    for (int i = 0; i < 3; i++)
                        Owner.AddAbility(ABattle.Instance.GetRandomAbilities(Owner, ABattle.Instance.GetCategory(Owner), 1)[0]);

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
