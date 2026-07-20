using System.Collections.Generic;
using Exiled.API.Features.Roles;
using MEC;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("응당한 대가", "즉시 55의 경험치를 얻습니다. 대신 20초 동안 전력을 사용할 수 없습니다.", AbilityCategory.Common, AbilityType.COMMON_SCP079_JUSTPRICE, RoleAbility.Scp079)]
public class JustPrice : Ability
{
    public override void OnEnabled()
    {
        IEnumerator<float> enumerator()
        {
            if (Owner.Role is Scp079Role scp079)
                scp079.AddExperience(55);

            for (int i = 0; i < 20; i++)
            {
                if (Owner.Role is Scp079Role scp079_)
                    scp079_.Energy = 0;

                yield return Timing.WaitForSeconds(1f);
            }
        }

        Timing.RunCoroutine(enumerator());
    }

    public override void OnDisabled()
    {
    }
}
