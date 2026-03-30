using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.Patches.Events.Scp049;
using MEC;
using RGM.Modes;
using System.Collections.Generic;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("Install", "80초 간 10초마다 10씩 경험치를 얻습니다.", RankAbilityType.Install, RankCategory.SCP_079, RankAbilityCategory.변칙성, "↕")]
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
                for (int i = 0; i < 8; i++)
                {
                    scp079.AddExperience(10);

                    yield return Timing.WaitForSeconds(10);
                }
            }
        }
    }
}
