using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes.Abilities.Unique.Scp106.Epic;

[Ability("회귀", "한 번의 공격으로 대상을 즉시 차원 주머니로 보냅니다.", 
    AbilityCategory.Epic, AbilityType.EPIC_SCP106_RETURN, RoleAbility.Scp106)]

public class Return : Ability
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
        if (Owner.Role is not Scp106Role scp106) return;
        if (ev.Attacker == null || ev.Attacker.ReferenceHub != scp106.Owner.ReferenceHub) return;
        if (ev.DamageHandler.Type != DamageType.Scp106) return;

        ev.Player.EnableEffect(EffectType.PocketCorroding, 1);
    }
}