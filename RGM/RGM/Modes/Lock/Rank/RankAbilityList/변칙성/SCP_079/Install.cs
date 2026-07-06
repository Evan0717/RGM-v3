using Exiled.API.Features.Roles;
using MEC;
using RGM.Modes;
using System.Collections.Generic;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("Install", "10초마다 20씩 경험치를 얻습니다.(최대 6회)", RankAbilityType.Install, RankCategory.SCP_079, RankAbilityCategory.변칙성, "↕")]
    public class Install : RankAbility
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
            if (Owner.Role is Scp079Role scp079)
            {
                for (int i = 0; i < 6; i++)
                {
                    scp079.AddExperience(20);

                    yield return Timing.WaitForSeconds(10);
                }
            }
        }
    }
}
