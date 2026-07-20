using MEC;

namespace RGM.Modes.Abilities.Unique.Scp096;

[Ability("원수", "분노 충전 시간이 50% 줄어듭니다.", AbilityCategory.Common, AbilityType.COMMON_SCP096_ENEMY, RoleAbility.Scp096)]
public class Enemy : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp096.Charging += OnCharging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp096.Charging -= OnCharging;
    }

    public void OnCharging(Exiled.Events.EventArgs.Scp096.ChargingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            ev.Scp096.RemainingChargeDuration /= 2;
        });
    }
}
