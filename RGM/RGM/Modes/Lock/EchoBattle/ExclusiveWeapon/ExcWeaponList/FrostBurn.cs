using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace RGM.Modes.ExclusiveWeapon;

/// <summary>
/// Frost Burn.
/// Passive: Attack 8%+(res*2%). On hit apply Slowness stacks at most once every 0.5s, clear after 5s no-hit.
/// At 10 stacks: Ensnared for 2+(0.4*res) seconds and prevent further stacks for 6 seconds.
/// </summary>
[ExclusiveWeapon(
    "서리",
    """
    공격력 8% + (공진 수치 * 2%) 증가. 적 타격 시 0.5초마다 적응형 이동속도 (-2 * 공진 수치)만큼 중첩(5초 미타격 시 해제).
    10스택 시 2초 + (0.4초 * 공진 수치) 속박, 이후 6초간 중첩 불가.
    """,
    ExclusiveWeaponType.FrostBurn)]
public class FrostBurn : ExcWeapon
{
    public override float AttackFlatMin => 4.7f;
    public override float AttackFlatMax => 58.7f;
    public override ExclusiveWeaponSecondaryStat SecondaryStat => ExclusiveWeaponSecondaryStat.CriticalChance;
    public override float SecondaryStatMin => 10.4f;
    public override float SecondaryStatMax => 29.3f;

    public override float PassiveAttackPercent => 8f + Resonance * 2f;

    private const float StackIntervalSeconds = 0.5f;
    private const float DecaySeconds = 5f;
    private const float PostRootStackLockoutSeconds = 6f;
    private const int MaxStacks = 10;

    private class FrostState
    {
        public int Stacks;
        public float LastHitAt;

        /// <summary>현재 대상의 이동속도에 반영되어 있는 서리 이동속도 감소량.</summary>
        public int AppliedReduction;

        public CoroutineHandle DecayHandle;
    }

    private readonly Dictionary<uint, FrostState> _states = new();
    private readonly Dictionary<uint, float> _stackLockouts = new();

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Player.Died += OnDied;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Exiled.Events.Handlers.Player.Died -= OnDied;

        foreach (var pair in _states)
        {
            Timing.KillCoroutines(pair.Value.DecayHandle);
            var target = Player.Get(pair.Key);
            ClearFrost(target, pair.Value);
        }

        _states.Clear();
        _stackLockouts.Clear();
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != Owner || Owner == null)
            return;

        if (ev.DamageHandler == null || ev.DamageHandler.Damage <= 0f)
            return;

        if (!HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        ApplyFrost(ev.Player);
    }

    // 사망 시 효과는 이미 초기화되므로, 되돌릴 이동속도 없이 서리 상태만 제거한다.
    private void OnDied(DiedEventArgs ev)
    {
        if (ev.Player == null)
            return;

        var id = ev.Player.NetId;
        if (_states.TryGetValue(id, out var state))
        {
            Timing.KillCoroutines(state.DecayHandle);
            _states.Remove(id);
        }

        _stackLockouts.Remove(id);
    }

    private void ApplyFrost(Player target)
    {
        if (target == null || !target.IsAlive)
            return;

        var id = target.NetId;
        if (_stackLockouts.TryGetValue(id, out var lockoutUntil))
        {
            if (Time.time < lockoutUntil)
                return;

            _stackLockouts.Remove(id);
        }

        if (!_states.TryGetValue(id, out var state))
        {
            state = new FrostState();
            _states[id] = state;
        }

        if (state.Stacks > 0 && Time.time - state.LastHitAt < StackIntervalSeconds)
            return;

        Timing.KillCoroutines(state.DecayHandle);

        state.Stacks = Mathf.Min(MaxStacks, state.Stacks + 1);
        state.LastHitAt = Time.time;

        if (state.Stacks < MaxStacks)
            SetSpeedReduction(target, state, state.Stacks * 2 * Resonance);

        if (state.Stacks >= MaxStacks)
        {
            var rootDuration = 2f + 0.4f * Resonance;
            target.EnableEffect(EffectType.Ensnared, 1, rootDuration);
            ClearFrost(target, state);
            _states.Remove(id);
            _stackLockouts[id] = Time.time + PostRootStackLockoutSeconds;
            EchoBattleCore.ShowNotification(
                Owner,
                $"<color=#66ccff>서리 속박</color> {rootDuration:0.#}초",
                1.5f);
            return;
        }

        state.DecayHandle = Timing.CallDelayed(DecaySeconds, () =>
        {
            if (!_states.TryGetValue(id, out var current) || current != state)
                return;

            if (Time.time - current.LastHitAt < DecaySeconds - 0.05f)
                return;

            var p = Player.Get(id);
            ClearFrost(p, current);
            _states.Remove(id);
        });
    }

    private static void ClearFrost(Player target, FrostState state)
    {
        SetSpeedReduction(target, state, 0);

        state.Stacks = 0;
        state.AppliedReduction = 0;
    }

    /// <summary>
    /// 서리 이동속도 감소량을 <paramref name="reduction"/>으로 맞춘다.
    /// 이전에 적용한 감소량과의 차이만큼만 현재 이동속도(MovementBoost - Slowness)에 반영한다.
    /// </summary>
    private static void SetSpeedReduction(Player target, FrostState state, int reduction)
    {
        EchoSpeed.Reduce(target, reduction - state.AppliedReduction);
        state.AppliedReduction = reduction;
    }
}
