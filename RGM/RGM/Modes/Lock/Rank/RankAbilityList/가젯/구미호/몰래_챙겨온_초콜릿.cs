using Exiled.API.Enums;
using MEC;
using RGM.Modes;
using System.Collections.Generic;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("몰래 챙겨온 초콜릿", "속도가 5% 느려지는 대신, 체력이 3초 동안 12씩 회복합니다.", RankAbilityType.몰래_챙겨온_초콜릿, RankCategory.구미호, "🍫")]
    public class 몰래_챙겨온_초콜릿 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            Owner.AddEffect(EffectType.Slowness, 5, 3);

            IEnumerator<float> enumerator()
            {
                for (int i = 0; i < 3; i++)
                {
                    Owner.Heal(12);

                    yield return Timing.WaitForSeconds(1f);
                }
            }

            Timing.RunCoroutine(enumerator());
        }
    }
}
