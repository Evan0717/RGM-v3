using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp939;

[Ability("흡혈 발톱", "할퀴기로 공격 시 25의 HS가 회복됩니다(1000 초과 시 회복량이 90% 감소합니다).", AbilityCategory.Scp939, AbilityType.SCP939_VAMPIRECLAW)]
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
        if (ev.Attacker.ReferenceHub != Owner.ReferenceHub || ev.Player != Owner || ev.Player == null) return;
        
        if (Owner.Role is not Scp939Role scp939) return;
        if ( scp939.Owner.HumeShield < 1000)
            scp939.Owner.HumeShield += 25;
        else
            scp939.Owner.HumeShield += 1;
    }
}
