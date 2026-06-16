using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp3114;

[Ability("숙련된 암살자", "교살 데미지가 6배 증가합니다.", AbilityCategory.Scp3114, AbilityType.SCP3114_SKILLEDASSASSIN)]
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
            ev.DamageHandler.Damage *= 6;
    }
}
