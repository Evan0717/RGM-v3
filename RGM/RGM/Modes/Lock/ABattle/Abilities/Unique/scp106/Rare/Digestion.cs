using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp106.Rare;

[Ability("소화", "처치한 대상을 『소화』 합니다.", 
    AbilityCategory.Rare, AbilityType.RARE_SCP106_DIGESTION, RoleAbility.Scp106)]

public class Digestion : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
    }
    
    private void OnDying(DyingEventArgs ev)
    {
        if (ev.Attacker == null || ev.Attacker != Owner) return;

        if (ev.Attacker.IsScpRole())
        {
            ev.Attacker.Health *= 1.01f;
            ev.Attacker.MaxHumeShield *= 1.01f;
        }
        else
        {
            ev.Attacker.Health *= 1.02f;
        }
    }
}