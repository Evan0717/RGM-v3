using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp939;

[Ability("흡혈 발톱", "할퀴기로 공격 시 50의 HS가 회복됩니다.(최대 1250까지 적용)", AbilityCategory.Common, AbilityType.COMMON_SCP939_VAMPIRECLAW, RoleAbility.Scp939)]
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
        if (ev.Attacker.ReferenceHub == scp939.Owner.ReferenceHub && scp939.Owner.HumeShield < 1250)
            scp939.Owner.HumeShield += 50;
    }
}
