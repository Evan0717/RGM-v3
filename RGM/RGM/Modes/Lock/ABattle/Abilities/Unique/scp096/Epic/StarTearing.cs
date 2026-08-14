using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Unique.Scp096.Epic;

[Ability("별자리 찢기", "SCP-096의 기본 공격 시, 51% 확률로 공격한 대상을 즉사시킵니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP096_STARTEARING, RoleAbility.Scp096)]
public class StarTearing : Ability
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
        if (Owner.Role is not Scp096Role scp096) return;
        if (ev.Attacker == null || ev.Attacker.ReferenceHub != scp096.Owner.ReferenceHub) return;
        if (ev.DamageHandler.Type != DamageType.Scp096) return;
        if (UnityEngine.Random.Range(1, 101) > 51) return;
        if (GodModePlayers.Contains(ev.Player))
            GodModePlayers.Remove(ev.Player);

        ev.Player.Hit(ev.Attacker, ev.Player.MaxHealth);
    }
}
