using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using RGM.Modes.Abilities.Legend;
using RGM.Modes.Abilities.Synergy;

namespace RGM.Modes.Abilities.Epic;

[Ability("거북 도사", "받는 모든 데미지는 40을 넘을 수 없습니다.(일부 피해 제외)", AbilityCategory.Epic, AbilityType.EPIC_TURTLE)]
public class Turtle : Ability
{
    private const float MaxDamage = 40f;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner ||
            ev.DamageHandler.Type == DamageType.Crushed ||
            WeakPointAttack.ShouldIgnoreDefenses(ev.Attacker) ||
            ZeroRule.ShouldIgnoreDefenses(ev))
            return;

        if (ev.IsInstantKill)
        {
            ev.IsAllowed = false;
            ev.Player.Hurt(MaxDamage, ev.DamageHandler.Type);
            
            return;
        }

        if (ev.DamageHandler.Damage > MaxDamage) {
            ev.DamageHandler.Damage = MaxDamage;
        }
    }
}