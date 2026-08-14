using System;
using Exiled.Events.EventArgs.Player;
using RGM.API.DataBases;
using RGM.Modes.Abilities.Synergy;

namespace RGM.Modes.Abilities.Normal;

[Ability("껍데기", "방어력이 1p 증가합니다.", AbilityCategory.Common, AbilityType.NORMAL_SHELL)]
public class Shell : Ability
{
    private const float DefenseFlat = 1f;

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
            Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type) ||
            WeakPointAttack.ShouldIgnoreDefenses(ev.Attacker))
            return;

        if (ABattle.Instance.GetAbility(Owner, AbilityType.NORMAL_SHELL) != this)
            return;

        float reduction = DefenseFlat * Owner.AbilityCount(AbilityType.NORMAL_SHELL);
        ev.DamageHandler.Damage = Math.Max(0f, ev.DamageHandler.Damage - reduction);
    }
}
