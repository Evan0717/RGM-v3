using Exiled.API.Enums;
using Exiled.Events.EventArgs.Scp0492;

namespace RGM.Modes.Abilities.Unique.Scp0492;

[Ability("당혹감", "목표물로 삼은 인간은 실명합니다.", AbilityCategory.Scp0492, AbilityType.SCP0492_CONFUSION)]
public class Confusion : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp0492.TriggeringBloodlust += OnTriggeringBloodlust;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp0492.TriggeringBloodlust -= OnTriggeringBloodlust;
    }

    public void OnTriggeringBloodlust(TriggeringBloodlustEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        ev.Target.EnableEffect(EffectType.Blurred, 1, 0.3f);
    }
}
