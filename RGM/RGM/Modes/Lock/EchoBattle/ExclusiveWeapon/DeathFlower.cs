using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.Modes;
using System.Collections.Generic;
using UnityEngine;

namespace RGM.Modes.ExclusiveWeapon;

/// <summary>
/// Death Flower.
/// Passive: HP 12%+(res*4%). On lethal hit: invuln/invis/speed for (1s*res), max 3 times.
/// On trigger: heal MaxHP * (10% + 5%*res).
/// </summary>
[ExclusiveWeapon(
    "\uD53C\uC548\uD654",
    "\uACF5\uACA9\uB825 Flat + HP%. \uCE58\uBA85\uD0C0 \uC0DD\uC874(\uBB34\uC801/\uD22C\uBA85/\uC774\uC18D), \uCD5C\uB300 3\uD68C.",
    ExclusiveWeaponType.DeathFlower)]
public class DeathFlower : ExcWeapon
{
    public override float AttackFlatMin => 5.7f;
    public override float AttackFlatMax => 71.4f;
    public override ExclusiveWeaponSecondaryStat SecondaryStat => ExclusiveWeaponSecondaryStat.HpPercent;
    public override float SecondaryStatMin => 13.2f;
    public override float SecondaryStatMax => 59.6f;

    public override float PassiveHpPercent => 12f + Resonance * 4f;

    const int MaxTriggers = 3;

    int _triggersUsed;
    bool _invulnerable;

    public override void OnEnabled()
    {
        _triggersUsed = 0;
        _invulnerable = false;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Timing.KillCoroutines($"DeathFlower_{Owner?.UserId}");
        _invulnerable = false;
        _triggersUsed = 0;
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner || Owner == null || !Owner.IsAlive)
            return;

        if (_invulnerable)
        {
            ev.IsAllowed = false;
            ev.DamageHandler.Damage = 0f;
            return;
        }

        if (_triggersUsed >= MaxTriggers)
            return;

        float damage = ev.DamageHandler?.Damage ?? 0f;
        if (damage <= 0f && !ev.IsInstantKill)
            return;

        float totalHp = Owner.Health + Owner.ArtificialHealth + Owner.HumeShield;
        bool lethal = ev.IsInstantKill || damage >= totalHp;
        if (!lethal)
            return;

        _triggersUsed++;
        ev.IsAllowed = false;
        ev.DamageHandler.Damage = 0f;

        float duration = 1f * Resonance;
        float speedBonus = 16f + 4f * Resonance;
        float heal = Owner.MaxHealth * (0.10f + 0.05f * Resonance);

        float room = Owner.MaxHealth - Owner.Health;
        if (room > 0f)
            Owner.Heal(Mathf.Min(heal, room));

        _invulnerable = true;
        Owner.EnableEffect(EffectType.Invisible, 1, duration);
        Owner.EnableEffect(EffectType.Ghostly, 1, duration);
        Owner.EnableEffect(EffectType.MovementBoost, (byte)Mathf.Clamp(Mathf.RoundToInt(speedBonus), 1, 255), duration);

        Timing.KillCoroutines($"DeathFlower_{Owner.UserId}");
        Timing.RunCoroutine(InvulnerabilityRoutine(duration), $"DeathFlower_{Owner.UserId}");

        Owner.ShowHint(
            $"<color=#ff6699>\uD53C\uC548\uD654</color> \uBC1C\uB3D9 ({_triggersUsed}/{MaxTriggers}) \u00B7 {duration:0.#}\uCD08 \uBB34\uC801",
            2f);
    }

    IEnumerator<float> InvulnerabilityRoutine(float duration)
    {
        yield return Timing.WaitForSeconds(duration);
        _invulnerable = false;
    }
}
