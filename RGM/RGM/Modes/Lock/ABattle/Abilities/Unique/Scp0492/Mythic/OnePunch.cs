using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Unique.Scp0492.Mythic;

[Ability("ONE PUNCH MAN",
    """
    자신의 일반 공격은 적을 즉사시킵니다.
    추가로, [전설] 스피드왜건, [영웅] 거북 도사, [전용 희귀] 급식, [전용 희귀] 보호막 능력을 획득합니다.
    """,
    AbilityCategory.Mythic,
    AbilityType.MYTHIC_SCP0492_ONEPUNCH,
    RoleAbility.Scp0492)]

public class OnePunch : Ability
{
    public override void OnEnabled()
    {
        Owner.AddAbility(AbilityType.LEGEND_SPEEDWAGON);
        Owner.AddAbility(AbilityType.EPIC_TURTLE);
        Owner.AddAbility(AbilityType.RARE_SCP0492_SHIELD);
        Owner.AddAbility(AbilityType.RARE_SCP0492_MEALS);
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != Owner ||
            ev.Player == Owner ||
            ev.DamageHandler.Type != DamageType.Scp0492)
            return;
        
        if (GodModePlayers.Contains(ev.Player))
            GodModePlayers.Remove(ev.Player);

        ev.Player.Hit(ev.Attacker, ev.Player.MaxHealth);
    }
}