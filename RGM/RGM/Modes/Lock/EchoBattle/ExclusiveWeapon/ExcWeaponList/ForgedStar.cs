using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using RGM.Modes;
using System.Collections.Generic;
using UnityEngine;

namespace RGM.Modes.ExclusiveWeapon;

/// <summary>
/// Forged Star.
/// Passive: Attack 11%+(res*2%). Burned target crit damage +(12%*res).
/// Flame stacks (2s CD, 8s duration). 6 stacks explode + 7m splash 60%.
/// </summary>
[ExclusiveWeapon(
    "위조된 작은 별",
    "공격력 11% + (공진 수치 * 2%) 증가. 화상 대상 공격 시 크리티컬 데미지 (12% * 공진 수치) 증가. 불꽃 7스택 폭발 + 8.5m 스플래시(85%).",
    ExclusiveWeaponType.ForgedStar)]
public class ForgedStar : ExcWeapon
{
    public override float AttackFlatMin => 2.5f;
    public override float AttackFlatMax => 31.3f;
    public override ExclusiveWeaponSecondaryStat SecondaryStat => ExclusiveWeaponSecondaryStat.CriticalChance;
    public override float SecondaryStatMin => 13.0f;
    public override float SecondaryStatMax => 41.0f;

    public override float PassiveAttackPercent => 11f + Resonance * 2f;

    const float FlameCooldown = 1f;
    const float FlameDuration = 8f;
    const int FlameMaxStacks = 7;
    const float SplashRadius = 8.5f;
    const float SplashRatio = 0.85f;

    class FlameState
    {
        public int Stacks;
        public float ExpireAt;
    }

    readonly Dictionary<uint, FlameState> _flames = new();
    float _nextFlameAt;
    bool _exploding;

    public override float GetCriticalDamageBonus(Player target)
    {
        if (target == null)
            return 0f;

        var burned = target.GetEffect(EffectType.Burned);
        if (burned != null && burned.IsEnabled)
            return 12f * Resonance;

        return 0f;
    }

    public override void OnEnabled()
    {
        _flames.Clear();
        _nextFlameAt = 0f;
        _exploding = false;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        _flames.Clear();
        _exploding = false;
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (_exploding || ev.Attacker != Owner || Owner == null)
            return;

        if (ev.DamageHandler == null || ev.DamageHandler.Damage <= 0f)
            return;

        if (!HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        if (EchoStats.AreAttackModifiersIgnored(Owner))
            return;

        TryAddFlame(ev.Player);
    }

    void TryAddFlame(Player target)
    {
        if (target == null || !target.IsAlive)
            return;

        float now = Time.time;
        if (now < _nextFlameAt)
        {
            RefreshDuration(target, now);
            return;
        }

        _nextFlameAt = now + FlameCooldown;

        uint id = target.NetId;
        if (!_flames.TryGetValue(id, out var state) || now > state.ExpireAt)
            state = new FlameState();

        state.Stacks = Mathf.Min(FlameMaxStacks, state.Stacks + 1);
        state.ExpireAt = now + FlameDuration;
        _flames[id] = state;

        if (state.Stacks >= FlameMaxStacks)
        {
            _flames.Remove(id);
            Explode(target);
        }
    }

    void RefreshDuration(Player target, float now)
    {
        uint id = target.NetId;
        if (_flames.TryGetValue(id, out var state) && now <= state.ExpireAt)
            state.ExpireAt = now + FlameDuration;
    }

    void Explode(Player primary)
    {
        if (primary == null || Owner == null)
            return;

        float explosion =
            50f
            + 10f * Resonance
            + primary.MaxHealth * (0.1f + 0.02f * Resonance);

        DealDamage(primary, explosion);

        Vector3 origin = primary.Position;
        foreach (var other in Player.List)
        {
            if (other == null || !other.IsAlive || other == primary || other == Owner)
                continue;

            if (!HitboxIdentity.IsEnemy(Owner.ReferenceHub, other.ReferenceHub))
                continue;

            if (Vector3.Distance(origin, other.Position) > SplashRadius)
                continue;

            DealDamage(other, explosion * SplashRatio);
        }

        EchoBattleCore.ShowNotification(
            Owner,
            $"<color=#ff8844>불꽃 폭발</color> {explosion:0.#}",
            1.5f);
    }

    void DealDamage(Player target, float amount)
    {
        if (target == null || !target.IsAlive || amount <= 0f)
            return;

        _exploding = true;
        try
        {
            target.Hurt(amount: amount, damageType: DamageType.Explosion, attacker: Owner);
        }
        finally
        {
            _exploding = false;
        }
    }
}
