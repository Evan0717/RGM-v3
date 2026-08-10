using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp3114;

[Ability("숙련된 암살자", "교살로 적을 즉시 처치할 수 있습니다.", AbilityCategory.Common, AbilityType.NORMAL_SCP3114_SKILLEDASSASSIN, RoleAbility.Scp3114)]
public class Minic : Ability
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
        if (ev.Attacker == null || ev.Attacker != Owner)
            return;

        if (ev.DamageHandler.Type == DamageType.Strangled)
            ev.DamageHandler.Damage *= 77.7f;
    }
}
