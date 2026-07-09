using Exiled.API.Features.Roles;
using MEC;
using RGM.Modes;
using System.Collections.Generic;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("자원봉사자", "의사의 부름이 흄쉴드를 초당 25씩 회복시킵니다.", RankAbilityType.자원봉사자, RankCategory.SCP_049, RankAbilityCategory.변칙성, "🆓")]
    public class 자원봉사자 : RankAbility
    {
        CoroutineHandle handle;

        public override void OnEnabled()
        {
            handle = Timing.RunCoroutine(enumerator());
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines(handle);
        }

        IEnumerator<float> enumerator()
        {
            if (Owner.Role is Scp049Role scp049)
            {
                while (true)
                {
                    if (scp049.IsCallActive)
                        Owner.HumeShield += 25;

                    yield return Timing.WaitForSeconds(1);
                }
            }
        }
    }
}
