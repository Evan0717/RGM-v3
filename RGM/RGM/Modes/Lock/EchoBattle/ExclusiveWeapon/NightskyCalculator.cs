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
    "\uBC24\uD558\uB298 \uC5F0\uC0B0 \uCE21\uC815\uAE30",
    "\uACF5\uACA9\uB825 Flat + HP%. AHP/HS \uD53C\uD574 \uC2DC \uCC28\uAC10\uB7C9\uC758 \uC77C\uBD80\uB9CC\uD07C HP \uD68C\uBCF5.",
    ExclusiveWeaponType.NightskyCalculator)]
public class NightskyCalculator : ExcWeapon
{
    public override float AttackFlatMin => 4.7f;
    public override float AttackFlatMax => 58.8f;
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
