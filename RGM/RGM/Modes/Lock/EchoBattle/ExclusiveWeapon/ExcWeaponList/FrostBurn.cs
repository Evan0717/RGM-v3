using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.Modes;
using System.Collections.Generic;
using UnityEngine;

namespace RGM.Modes.ExclusiveWeapon;

/// <summary>
/// Frost Burn.
/// Passive: Attack 8%+(res*2%). On hit apply Slowness stacks, clear after 3s no-hit.
/// At 10 stacks: Ensnared for 3+(0.6*res) seconds.
/// </summary>
[ExclusiveWeapon(
    "서리",
    "공격력 8% + (공진 수치 * 2%) 증가. 적 타격 시 Slowness를 1% + (1% * 공진 수치) 중첩(3초 미타격 시 해제). 10스택 시 3초 + (0.6초 * 공진 수치) 속박.",
    ExclusiveWeaponType.FrostBurn)]
public class FrostBurn : ExcWeapon
{
    public override float AttackFlatMin => 6.8f;
    public override float AttackFlatMax => 83.9f;
    public override ExclusiveWeaponSecondaryStat SecondaryStat => ExclusiveWeaponSecondaryStat.CriticalChance;
    public override float SecondaryStatMin => 5.4f;
    public override float SecondaryStatMax => 24.3f;

    public override float PassiveAttackPercent => 8f + Resonance * 2f;

    const float DecaySeconds = 3f;
    const int MaxStacks = 10;

    class FrostState
    {
        public int Stacks;
        public float LastHitAt;
        public byte AppliedSlowness;
        public CoroutineHandle DecayHandle;
    }

    readonly Dictionary<uint, FrostState> _states = new();

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;

        foreach (var pair in _states)
        {
            Timing.KillCoroutines(pair.Value.DecayHandle);
            var target = Player.Get(pair.Key);
            ClearFrost(target, pair.Value);
        }

        _states.Clear();
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != Owner || Owner == null)
            return;

        if (ev.DamageHandler == null || ev.DamageHandler.Damage <= 0f)
            return;

        if (!HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        ApplyFrost(ev.Player);
    }

    void ApplyFrost(Player target)
    {
        if (target == null || !target.IsAlive)
            return;

        uint id = target.NetId;
        if (!_states.TryGetValue(id, out var state))
        {
            state = new FrostState();
            _states[id] = state;
        }

        Timing.KillCoroutines(state.DecayHandle);

        int perStack = 1 + Resonance;
        state.Stacks = Mathf.Min(MaxStacks, state.Stacks + 1);
        state.LastHitAt = Time.time;

        if (state.AppliedSlowness > 0)
            target.DisableEffect(EffectType.Slowness);

        state.AppliedSlowness = (byte)Mathf.Clamp(state.Stacks * perStack, 1, 255);
        target.EnableEffect(EffectType.Slowness, state.AppliedSlowness, DecaySeconds + 0.1f);

        if (state.Stacks >= MaxStacks)
        {
            float rootDuration = 3f + 0.6f * Resonance;
            target.EnableEffect(EffectType.Ensnared, 1, rootDuration);
            ClearFrost(target, state);
            _states.Remove(id);
            Owner.ShowHint($"<color=#66ccff>서리 속박</color> {rootDuration:0.#}초", 1.5f);
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

    static void ClearFrost(Player target, FrostState state)
    {
        if (target != null && state.AppliedSlowness > 0)
            target.DisableEffect(EffectType.Slowness);

        state.Stacks = 0;
        state.AppliedSlowness = 0;
    }
}
