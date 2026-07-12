using System.Collections.Generic;
using MEC;

namespace RGM.Modes.Abilities.Rare;

[Ability("육체 강화", "1초당 1HP를 받습니다.", AbilityCategory.Rare, AbilityType.RARE_PHYSICALSTRENGTHENING)]
public class PhysicalStrengthening : Ability
{
    CoroutineHandle _upgradeBody;

    public override void OnEnabled()
    {
        _upgradeBody = Timing.RunCoroutine(UpgradeBody());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_upgradeBody);
    }

    public IEnumerator<float> UpgradeBody()
    {
        while (true)
        {
            if (Owner.MaxHealth > Owner.Health)
                Owner.Health += 1;

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
