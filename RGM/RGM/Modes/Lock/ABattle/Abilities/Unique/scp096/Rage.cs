using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp096;

[Ability("격노", "분노 시 받는 피해가 25% 줄어듭니다.", AbilityCategory.Scp096, AbilityType.SCP096_RAGE)]
public class Rage : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (Owner.Role is Scp096Role scp096)
        {
            if (scp096.RageManager.IsEnraged)
            {
                ev.Amount *= 0.75f;
            }
        }
    }
}
