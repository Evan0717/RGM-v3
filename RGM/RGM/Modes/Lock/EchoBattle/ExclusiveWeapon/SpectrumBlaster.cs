using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using RGM.Modes;
using UnityEngine;

namespace RGM.Modes.ExclusiveWeapon;

/// <summary>
/// Spectrum Blaster.
/// Passive: Attack 8%+(res*2%). Every 10 hits, next attack pierce damage mult (10%*res).
/// Uses damage multiplier instead of native ArmorPenetration.
/// </summary>
[ExclusiveWeapon(
    "\uC2A4\uD399\uD2B8\uB7FC \uBE14\uB798\uC2A4\uD130",
    "\uACF5\uACA9\uB825 Flat + \uD06C\uB9AC\uD2F0\uCEEC. 10\uD68C \uD0C0\uACA9\uB9C8\uB2E4 \uB2E4\uC74C \uACF5\uACA9\uC5D0 \uAD00\uD1B5(\uB370\uBBF8\uC9C0 \uBC30\uC728) \uBD80\uC5EC.",
    ExclusiveWeaponType.SpectrumBlaster)]
public class SpectrumBlaster : ExcWeapon
{
    public override float AttackFlatMin => 6.8f;
    public override float AttackFlatMax => 83.9f;
    public override ExclusiveWeaponSecondaryStat SecondaryStat => ExclusiveWeaponSecondaryStat.CriticalChance;
    public override float SecondaryStatMin => 5.4f;
    public override float SecondaryStatMax => 24.3f;

    public override float PassiveAttackPercent => 8f + Resonance * 2f;

    int _hitCount;
    bool _pierceArmed;

    public override void OnEnabled()
    {
        _hitCount = 0;
        _pierceArmed = false;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        _hitCount = 0;
        _pierceArmed = false;
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != Owner || Owner == null)
            return;

        if (ev.DamageHandler == null || ev.DamageHandler.Damage <= 0f)
            return;

        if (!HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        if (_pierceArmed)
        {
            float pierce = 0.10f * Resonance;
            ev.DamageHandler.Damage *= 1f + pierce;
            _pierceArmed = false;
        }

        _hitCount++;
        if (_hitCount >= 10)
        {
            _hitCount = 0;
            _pierceArmed = true;
            Owner.ShowHint($"<color=#88aaff>\uC2A4\uD399\uD2B8\uB7FC</color> \uB2E4\uC74C \uACF5\uACA9 \uAD00\uD1B5 +{10 * Resonance}%", 1.5f);
        }
    }
}
