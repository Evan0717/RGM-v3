using Exiled.Events.EventArgs.Scp173;

namespace RGM.Modes.Abilities.Unique.Scp173;

[Ability("괴이", "순간이동한 방이 1초 동안 정전됩니다.", AbilityCategory.Scp173, AbilityType.SCP173_ABERRATION)]
public class Aberration : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;
    }

    public void OnBlinking(BlinkingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (!ev.Player.CurrentRoom.AreLightsOff)
            ev.Player.CurrentRoom.TurnOffLights(1 * ev.Player.AbilityCount(AbilityType.SCP173_ABERRATION));
    }
}
