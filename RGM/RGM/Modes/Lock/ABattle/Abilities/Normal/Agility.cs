using Exiled.Events.EventArgs.Player;
using RGM.API.DataBases;
using RGM.API.Features;
using RGM.Modes.Abilities.Synergy;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("민첩", "회피율이 5%p 증가합니다. SCP 진영은 기존의 40% 확률로 적용됩니다.", AbilityCategory.Common, AbilityType.NORMAL_AGILITY)]
public class Agility : Ability
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
        if (ev.Attacker == null || 
            ev.Player != Owner || 
            !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub) || 
            Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type) ||
            WeakPointAttack.ShouldIgnoreDefenses(ev.Attacker))
            return;

        int dodgeChance = Owner.IsScpRole() ? 2 : 5;

        if (Random.Range(1, 101) > dodgeChance) return;
        ev.IsAllowed = false;

        ev.Attacker.AddHint("이런, 미끄러져 버렸군요.", $"이런, 미끄러져 버렸군요.", 1.2f);
        ev.Player.AddHint("아슬아슬하게 회피했군요!", $"아슬아슬하게 회피했군요!", 1.2f);
    }
}