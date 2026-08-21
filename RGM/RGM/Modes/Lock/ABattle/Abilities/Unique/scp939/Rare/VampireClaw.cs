using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp939.Rare;

[Ability("흡혈 발톱", "발톱 공격으로 공격 시 60의 HS가 회복됩니다.(최대 2000까지 적용)",
    AbilityCategory.Rare, AbilityType.RARE_SCP939_VAMPIRECLAW, RoleAbility.Scp939)]
public class VampireClaw : Ability
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
        if (ev.DamageHandler.Type != DamageType.Scp939) return;
        if (ev.Attacker.ReferenceHub == scp939.Owner.ReferenceHub && scp939.Owner.HumeShield < 2000)
            scp939.Owner.HumeShield += 60;
    }
}
