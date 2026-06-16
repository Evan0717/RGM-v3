using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp939;

[Ability("흡혈 발톱", "공격 시 40의 HS가 회복됩니다.", AbilityCategory.Scp939, AbilityType.SCP939_VAMPIRECLAW)]
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
        if (ev.Attacker.ReferenceHub == scp939.Owner.ReferenceHub && scp939.Owner.HumeShield < 700)
            scp939.Owner.HumeShield += 25;
        else
            scp939.Owner.HumeShield += 1;
    }
}
