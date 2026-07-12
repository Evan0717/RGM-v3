using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace RGM.Modes.Abilities.Epic;

[Ability("극독", "죽인 자에게 60초 동안 심장 마비와 중독 효과를 부여합니다.", AbilityCategory.Epic, AbilityType.EPIC_EXTREMEPOISON)]
public class ExtremePoison : Ability
{
    const float Duration = 60f;
    
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner || ev.Attacker == null)
            return;

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (Owner.IsDead)
            {
                ev.Attacker.EnableEffect(EffectType.CardiacArrest, 1,
                    Duration * Owner.AbilityCount(AbilityType.EPIC_EXTREMEPOISON));
                ev.Attacker.EnableEffect(EffectType.Poisoned, 1,
                    Duration * Owner.AbilityCount(AbilityType.EPIC_EXTREMEPOISON));
            }
        });
    }
}