using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using UnityEngine;

namespace RGM.Modes.ExclusiveWeapon;

/// <summary>
/// Spectrum Blaster.
/// Passive: Attack 8%+(res*2%). On attacking an enemy, gain (1 * resonance)% movement speed every 0.8 seconds,
/// stacking up to 8 times. The effect lasts 5 seconds and refreshes on subsequent attacks.
/// </summary>
[ExclusiveWeapon(
    "스펙트럼 블래스터",
    "공격력 8% + (공진 수치 * 2%) 증가. 적 공격 시 0.8초 간격으로 이동속도 (1 * 공진 수치)%만큼 증가.\n최대 8스택 중첩, 5초 지속, 5초 내에 적 공격 시 효과 지속 시간 갱신.",
    ExclusiveWeaponType.SpectrumBlaster)]
public class SpectrumBlaster : ExcWeapon
{
    public override float AttackFlatMin => 3.0f;
    public override float AttackFlatMax => 36.7f;
    public override ExclusiveWeaponSecondaryStat SecondaryStat => ExclusiveWeaponSecondaryStat.CriticalChance;
    public override float SecondaryStatMin => 10.4f;
    public override float SecondaryStatMax => 29.3f;
    public override float PassiveAttackPercent => 8f + Resonance * 2f;

    const float StackIntervalSeconds = 0.8f;
    const float DurationSeconds = 5f;
    const int MaxStacks = 8;

    int _stacks;
    float _lastStackAt;
    float _lastHitAt;

    public override void OnEnabled()
    {
        _stacks = 0;
        _lastStackAt = 0f;
        _lastHitAt = 0f;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        _stacks = 0;
        _lastStackAt = 0f;
        _lastHitAt = 0f;
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (Owner == null || ev.Attacker != Owner)
            return;

        if (ev.DamageHandler == null || ev.DamageHandler.Damage <= 0f)
            return;

        if (!HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        if (EchoStats.AreAttackModifiersIgnored(Owner))
            return;

        float now = Time.time;
        if (now - _lastHitAt >= DurationSeconds)
        {
            _stacks = 0;
            _lastStackAt = 0f;
        }

        if (now - _lastStackAt >= StackIntervalSeconds)
        {
            _stacks = Mathf.Min(MaxStacks, _stacks + 1);
            _lastStackAt = now;
        }

        _lastHitAt = now;
        byte movementBoost = (byte)Mathf.Clamp(_stacks * Resonance, 1, byte.MaxValue);
        Owner.EnableEffect(EffectType.MovementBoost, movementBoost, DurationSeconds);
    }
}
