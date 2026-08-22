using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp939.Rare;

[Ability("출혈", "SCP-939의 기본 공격에 출혈 효과가 적용됩니다.",
    AbilityCategory.Rare, AbilityType.RARE_SCP939_BLEEDING, RoleAbility.Scp939)]

public class Bleeding : Ability
{
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
        if (Owner.Role is not Scp939Role scp939) return;
        if (ev.Attacker == null || ev.Attacker.ReferenceHub != scp939.Owner.ReferenceHub) return;
        if (ev.DamageHandler.Type != DamageType.Scp939) return;

        ev.Player.EnableEffect(EffectType.Bleeding, 1, 15f);
    }
}