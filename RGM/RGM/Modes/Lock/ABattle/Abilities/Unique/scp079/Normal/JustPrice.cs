using System.Collections.Generic;
using Exiled.API.Features.Roles;
using MEC;

namespace RGM.Modes.Abilities.Unique.Scp079.Common;

[Ability("응당한 대가", "20초동안 전력을 사용할 수 없지만, 그 후 12초마다 10의 경험치를 획득합니다.", AbilityCategory.Normal, AbilityType.NORMAL_SCP079_JUSTPRICE, RoleAbility.Scp079)]
public class JustPrice : Ability
{
    CoroutineHandle JustPriceHandle;
    public override void OnEnabled()
    {
        IEnumerator<float> enumerator()
        {
            if (Owner.Role is Scp079Role scp079)
            {
                for (int i = 0; i < 20; i++)
                {
                    scp079.Energy = 0;
                    Timing.WaitForSeconds(1f);
                }

                while (true)
                {
                    scp079.AddExperience(10);
                    yield return Timing.WaitForSeconds(12f);
                }
            }
            
        }

        JustPriceHandle = Timing.RunCoroutine(enumerator());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(JustPriceHandle);
    }
}
