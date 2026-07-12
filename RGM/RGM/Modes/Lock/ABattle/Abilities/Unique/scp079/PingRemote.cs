using Exiled.Events.EventArgs.Scp079;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("핑 리모컨", "핑을 찍은 방이 1.5초 간 정전이 됩니다.", AbilityCategory.Scp079, AbilityType.SCP079_PINGREMOTE)]
public class PingRemote : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;
    }

    public void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (!ev.Room.AreLightsOff)
            ev.Room.TurnOffLights(1.5f * ev.Player.AbilityCount(AbilityType.SCP079_PINGREMOTE));
    }
}
