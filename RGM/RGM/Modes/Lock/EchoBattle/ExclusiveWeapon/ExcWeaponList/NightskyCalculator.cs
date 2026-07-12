using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using RGM.Modes;
using System.Collections.Generic;
using UnityEngine;

namespace RGM.Modes.ExclusiveWeapon;

/// <summary>
/// Nightsky Calculator.
/// Passive: HP 12%+(res*4%). On AHP/HS loss, heal HP by (12%*res) of pure shield loss.
/// </summary>
[ExclusiveWeapon(
    "밤하늘 연산 측정기",
    "HP 12% + (공진 수치 * 4%) 증가. AHP(또는 HS)가 피해를 입을 경우, AHP(HS)의 순수 차감량의 (12% * 공진 수치)만큼 HP 회복. 단, HP 최대치를 초과하여 회복할 수 없음.",
    ExclusiveWeaponType.NightskyCalculator)]
public class NightskyCalculator : ExcWeapon
{
    public override float AttackFlatMin => 3.3f;
    public override float AttackFlatMax => 41.2f;
    public override ExclusiveWeaponSecondaryStat SecondaryStat => ExclusiveWeaponSecondaryStat.HpPercent;
    public override float SecondaryStatMin => 16.0f;
    public override float SecondaryStatMax => 72.2f;

    public override float PassiveHpPercent => 12f + Resonance * 4f;

    readonly Dictionary<Player, (float Ahp, float Hs)> _before = new();

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Player.Hurt += OnHurt;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Exiled.Events.Handlers.Player.Hurt -= OnHurt;
        _before.Clear();
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner || Owner == null)
            return;

        if (ev.DamageHandler == null || ev.DamageHandler.Damage <= 0f)
            return;

        _before[Owner] = (Owner.ArtificialHealth, Owner.HumeShield);
    }

    void OnHurt(HurtEventArgs ev)
    {
        if (ev.Player != Owner || Owner == null || !Owner.IsAlive)
            return;

        if (!_before.TryGetValue(Owner, out var before))
            return;

        _before.Remove(Owner);

        float ahpLost = Mathf.Max(0f, before.Ahp - Owner.ArtificialHealth);
        float hsLost = Mathf.Max(0f, before.Hs - Owner.HumeShield);
        float pureShieldLost = ahpLost + hsLost;
        if (pureShieldLost <= 0f)
            return;

        float heal = pureShieldLost * (0.12f * Resonance);
        if (heal <= 0f)
            return;

        float room = Owner.MaxHealth - Owner.Health;
        if (room <= 0f)
            return;

        Owner.Heal(Mathf.Min(heal, room));
    }
}
