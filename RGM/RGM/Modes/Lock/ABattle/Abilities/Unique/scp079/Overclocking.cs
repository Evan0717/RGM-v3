using System.Collections.Generic;
using Exiled.API.Features.Roles;
using MEC;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("오버클럭", "1초마다 전력을 0.2 얻습니다.", AbilityCategory.Scp079, AbilityType.SCP079_OVERCLOCKING)]
public class Overclocking : Ability
{
    public override void OnEnabled()
    {
        IEnumerator<float> enumerator()
        {
            while (Owner.Role.Type == RoleTypeId.Scp079)
            {
                if (Owner.Role is Scp079Role scp079)
                    scp079.Energy += 0.2f;

                yield return Timing.WaitForSeconds(1f);
            }
        }

        Timing.RunCoroutine(enumerator());
    }

    public override void OnDisabled()
    {

    }
}
