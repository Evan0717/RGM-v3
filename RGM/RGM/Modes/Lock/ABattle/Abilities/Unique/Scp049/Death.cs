using System.Collections.Generic;
using MEC;

namespace RGM.Modes.Abilities.Unique.Scp049;

[Ability("사신", "공격 쿨타임이 35% 줄어듭니다.", AbilityCategory.Common, AbilityType.COMMON_SCP049_DEATH, RoleAbility.Scp049)]
public class Death : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp049.Attacking += OnScp049Attacking;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp049.Attacking -= OnScp049Attacking;
    }

    public IEnumerator<float> OnScp049Attacking(Exiled.Events.EventArgs.Scp049.AttackingEventArgs ev)
    {
        if (ev.Player != Owner)
            yield break;

        yield return Timing.WaitForOneFrame;

        ev.Scp049.RemainingAttackCooldown *= 0.65f;
    }
}
