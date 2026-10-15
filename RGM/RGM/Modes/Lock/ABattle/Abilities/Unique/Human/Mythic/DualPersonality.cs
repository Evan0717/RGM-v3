using System;
using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.ToggleableLights;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using RGM.Variables;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Human.Mythic;

[Ability("이중인격",
    """
    두 가지 형상, 환멸과 연극의 모습으로 변신합니다.
    지급된 아이템을 사용하여 인격을 변경할 수 있으며, 각 인격은 특정 종류의 능력치를 크게 강화시킵니다.
    """, 
    AbilityCategory.Mythic, AbilityType.MYTHIC_HUMAN_DUALPERSONALITY, RoleAbility.Human)]

public class DualPersonality : Ability
{
    private enum Form
    {
        /// <summary>연극의 형상 (Lantern ON)</summary>
        Drama,

        /// <summary>환멸의 형상 (Lantern OFF)</summary>
        Disillusion,
    }

    // 형상 전환
    private const float TransitionDuration = 1f;

    // 연극의 형상
    private const float DramaHealInterval = 0.5f;
    private const float DramaHealRatio = 0.07f;
    private const float DramaAhpSustain = 30f;
    private const int DramaSpeedReduction = 40;
    private const float DramaDamageMultiplier = 0.7f;
    private const float DramaLifestealRatio = 0.08f;

    // 환멸의 형상
    private const float DisillusionTickInterval = 1f;
    private const float DisillusionSelfDamageRatio = 0.02f;
    private const float DisillusionMinHealthRatio = 0.1f;
    private const int DisillusionBoostIntensity = 45;
    private const float DisillusionDamageMultiplier = 1.45f;

    private Form _form;
    private ushort _lanternSerial;
    private float _lockedUntil;

    private bool _isSpeedOverridden;
    private int _originalBoost;
    private int _originalSlowness;

    private CoroutineHandle _formRoutine;
    private CoroutineHandle _syncRoutine;

    public override void OnEnabled()
    {
        Item lantern = Owner.AddItem(ItemType.Lantern);
        _lanternSerial = lantern.Serial;

        Exiled.Events.Handlers.Player.TogglingFlashlight += OnTogglingFlashlight;
        Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Player.Hurt += OnHurt;

        // 능력 획득 시 기본 형상은 연극 (Lantern ON)
        _form = Form.Drama;
        _lockedUntil = 0f;
        SetLanternState(true);
        EnterForm(Form.Drama);

        _syncRoutine = Timing.RunCoroutine(SyncLanternRoutine());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.TogglingFlashlight -= OnTogglingFlashlight;
        Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
        Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Exiled.Events.Handlers.Player.Hurt -= OnHurt;

        Timing.KillCoroutines(_syncRoutine);
        ExitForm(_form);

        Item lantern = GetLantern();
        if (lantern != null)
            Owner.RemoveItem(lantern);
    }

    #region 형상 전환

    private void OnTogglingFlashlight(TogglingFlashlightEventArgs ev)
    {
        if (ev.Player != Owner || ev.Item?.Serial != _lanternSerial)
            return;

        // 혼동 방지를 위해 전환 직후에는 아이템 상호작용을 막는다.
        if (Time.time < _lockedUntil)
        {
            ev.IsAllowed = false;
            return;
        }

        SetForm(ev.NewState ? Form.Drama : Form.Disillusion);
    }

    /// <summary>
    /// 이벤트가 발생하지 않는 경우를 대비해 Lantern 상태와 형상을 동기화합니다.
    /// </summary>
    private IEnumerator<float> SyncLanternRoutine()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(0.1f);

            if (Owner == null || !Owner.IsAlive)
                continue;

            if (GetLantern()?.Base is not ToggleableLightItemBase light)
                continue;

            Form lanternForm = light.IsEmittingLight ? Form.Drama : Form.Disillusion;
            if (lanternForm == _form)
                continue;

            if (Time.time < _lockedUntil)
                SetLanternState(_form == Form.Drama);
            else
                SetForm(lanternForm);
        }
    }

    private void SetForm(Form next)
    {
        if (_form == next)
            return;

        ExitForm(_form);
        _form = next;
        EnterForm(next);

        ApplyTransitionEffects();
        _lockedUntil = Time.time + TransitionDuration + Timing.WaitForOneFrame;

        string formName = next == Form.Drama ? "연극" : "환멸";
        Owner.AddHint("이중인격",
            $"<color={ABattle.RatingColor["신화"]}>{formName}</color>의 형상으로 변신했습니다.");
    }

    private void EnterForm(Form form)
    {
        switch (form)
        {
            case Form.Drama:
                ApplyDramaSpeed();
                _formRoutine = Timing.RunCoroutine(DramaRoutine());
                break;

            case Form.Disillusion:
                Owner.AddEffect(EffectType.MovementBoost, DisillusionBoostIntensity);
                _formRoutine = Timing.RunCoroutine(DisillusionRoutine());
                break;
        }
    }

    private void ExitForm(Form form)
    {
        Timing.KillCoroutines(_formRoutine);

        switch (form)
        {
            case Form.Drama:
                RestoreSpeed();
                break;

            case Form.Disillusion:
                Owner.RemoveEffect(EffectType.MovementBoost, DisillusionBoostIntensity);
                break;
        }
    }

    /// <summary>
    /// 전환 시 생존 효과(무적 + 은신)를 부여하고, 동시에 이동을 막는다.
    /// 이동속도 보정과 충돌하지 않도록 『생존』의 이동속도 증가는 적용하지 않는다.
    /// </summary>
    private void ApplyTransitionEffects()
    {
        Owner.AddEffect(EffectType.Invisible, 1, TransitionDuration);
        Owner.AddEffect(EffectType.Ghostly, 1, TransitionDuration);
        Owner.ApplyGodMode(TransitionDuration);

        Owner.AddEffect(EffectType.SinkHole, 1, TransitionDuration);
        Owner.AddEffect(EffectType.Ensnared, 1, TransitionDuration);
    }

    #endregion

    #region 연극의 형상

    private IEnumerator<float> DramaRoutine()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(DramaHealInterval);

            if (Owner == null || !Owner.IsAlive)
                yield break;

            float amount = Owner.MaxHealth * DramaHealRatio;
            Owner.Heal(amount);

            // 회복량만큼 AHP와 최대 AHP를 함께 증가시킨다. (30초 유지 후 기본 감소량으로 감소)
            float limit = Owner.ArtificialHealth + amount;
            Owner.MaxArtificialHealth = Mathf.Max(Owner.MaxArtificialHealth, limit);
            Owner.AddAhp(amount, limit, sustain: DramaAhpSustain);
        }
    }

    /// <summary>
    /// 현재 이동속도 증감량(MovementBoost - Slowness)을 계산한 뒤 -40을 적용하여,
    /// 결과가 양수면 MovementBoost, 음수면 Slowness를 적용한다. (0이면 아무것도 적용하지 않음)
    /// </summary>
    private void ApplyDramaSpeed()
    {
        _originalBoost = GetIntensity(EffectType.MovementBoost);
        _originalSlowness = GetIntensity(EffectType.Slowness);
        _isSpeedOverridden = true;

        int speed = _originalBoost - _originalSlowness - DramaSpeedReduction;

        SetEffectIntensity(EffectType.MovementBoost, Math.Max(speed, 0));
        SetEffectIntensity(EffectType.Slowness, Math.Max(-speed, 0));
    }

    private void RestoreSpeed()
    {
        if (!_isSpeedOverridden)
            return;

        _isSpeedOverridden = false;

        if (Owner == null || !Owner.IsAlive)
            return;

        SetEffectIntensity(EffectType.MovementBoost, GetBaseIntensity(EffectType.MovementBoost, _originalBoost));
        SetEffectIntensity(EffectType.Slowness, GetBaseIntensity(EffectType.Slowness, _originalSlowness));
    }

    #endregion

    #region 환멸의 형상

    private IEnumerator<float> DisillusionRoutine()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(DisillusionTickInterval);

            if (Owner == null || !Owner.IsAlive)
                yield break;

            // 최대 체력의 10% 아래로는 감소하지 않는다.
            float minHealth = Owner.MaxHealth * DisillusionMinHealthRatio;
            float damage = Math.Min(Owner.MaxHealth * DisillusionSelfDamageRatio, Owner.Health - minHealth);

            if (damage > 0f)
                Owner.Health -= damage;
        }
    }

    #endregion

    #region 공격

    private void OnHurting(HurtingEventArgs ev)
    {
        if (!IsOwnerAttack(ev.Attacker, ev.Player, ev.DamageHandler.Type))
            return;

        ev.DamageHandler.Damage *= _form == Form.Drama ? DramaDamageMultiplier : DisillusionDamageMultiplier;
    }

    private void OnHurt(HurtEventArgs ev)
    {
        if (!IsOwnerAttack(ev.Attacker, ev.Player, ev.DamageHandler.Type))
            return;

        float damage = ev.DamageHandler.Damage;
        if (damage <= 0f)
            return;

        switch (_form)
        {
            case Form.Drama:
                Owner.Heal(damage * DramaLifestealRatio);
                break;

            case Form.Disillusion:
                // 잃은 HP만큼 관통 고정 피해를 추가로 입힌다.
                float lostHealth = Owner.MaxHealth - Owner.Health;
                if (lostHealth > 0f && ApplyFixedDamage.Apply(Owner, ev.Player, lostHealth))
                    Owner.ShowHitMarker(1.25f);
                break;
        }
    }

    private bool IsOwnerAttack(Exiled.API.Features.Player attacker, Exiled.API.Features.Player target, DamageType damageType)
    {
        return attacker != null &&
               attacker == Owner &&
               target != null &&
               target != Owner &&
               !ApplyFixedDamage.IsApplying &&
               !Datas.BlockDamageTypes.Contains(damageType) &&
               HitboxIdentity.IsEnemy(attacker.ReferenceHub, target.ReferenceHub);
    }

    #endregion

    #region Lantern

    private void OnDroppingItem(DroppingItemEventArgs ev)
    {
        if (ev.Player != Owner || ev.Item?.Serial != _lanternSerial)
            return;

        ev.IsAllowed = false;
        ev.Player.AddHint("이중인격", "형상을 변경하는 랜턴은 버릴 수 없습니다.", 1.5f);
    }

    private void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Player != Owner || ev.Item?.Serial != _lanternSerial)
            return;

        ev.Player.AddHint("이중인격",
            $"<b><color={ABattle.RatingColor["신화"]}>이중인격</color></b>의 랜턴입니다. 켜면 <b>연극</b>, 끄면 <b>환멸</b>의 형상이 됩니다.");
    }

    private Item GetLantern() => Owner?.Items.FirstOrDefault(x => x.Serial == _lanternSerial);

    private void SetLanternState(bool isOn)
    {
        if (GetLantern()?.Base is ToggleableLightItemBase light && light.IsEmittingLight != isOn)
            light.IsEmittingLight = isOn;
    }

    #endregion

    #region 효과 유틸

    private int GetIntensity(EffectType type)
    {
        StatusEffectBase effect = Owner.GetEffect(type);
        return effect is { IsEnabled: true } ? effect.Intensity : 0;
    }

    /// <summary>
    /// AddEffect로 누적 관리되는 값이 있으면 그 값을, 없으면 덮어쓰기 전에 저장한 값을 반환합니다.
    /// </summary>
    private int GetBaseIntensity(EffectType type, int fallback)
    {
        if (Variable.EffectIntensities.TryGetValue(Owner, out var intensities) &&
            intensities.TryGetValue(type, out int tracked))
            return Math.Min(tracked, byte.MaxValue);

        return fallback;
    }

    private void SetEffectIntensity(EffectType type, int intensity)
    {
        if (intensity <= 0)
            Owner.DisableEffect(type);
        else
            Owner.EnableEffect(type, (byte)Math.Min(intensity, byte.MaxValue));
    }

    #endregion

    /*
     * 이중인격 구현
     *
     * 1. 두 가지 형상(연극/환멸) 상태를 지정. 두 형상 간 전환 쿨타임은 1초로 설정. 형상 간 전환 때 마다 생존 효과 부여.
     * 추가로, 전환 때 마다 이동 불가.(Sinkhole, Ensnared 효과 부여)
     * 2. 능력 획득 시 기본 형상은 연극 형상으로 지정.
     * 3. 형상 간 전환은 Lantern으로 하며, 해당 아이템 내장 상호작용 기능(Lantern ON/OFF)을 통해 전환 가능하도록 구현. 해당 아이템은 버릴 수 없도록 설정.
     * 3-1. Lantern ON일 때 연극의 형상, OFF 일때 환멸의 형상으로 설정. 혼동을 방지하기 위해 전환 후 1 + OneFrame초 간 아이템 상호작용을 금지함.
     * 
     * 4. 연극의 형상 능력치
     * 4-1. 0.5초 간격으로 최대 HP의 7% 회복, 회복량 만큼 AHP가 증가하며, 최대 AHP도 동시에 증가.
     * 4-2. 해당 AHP는 30초간 지속되며, 30초가 지날 시 지속적으로 감소(감소량은 게임 기본 값 사용)
     * 4-3. 이동 속도 30% 감소
     * 4-3-1. 감속을 구현할 때, 현재 자신의 이동속도 증감량을 계산하여, 해당 값에서 -40을 적용. (단순히 이펙트를 적용하는 게 아님!)
     * 이동속도 증감량은 현재 Owner의 MovementBoost 또는 Slowness의 intensity 값을 가져와서 계산.
     * 증감량을 계산하였을 때, 양수면 MovementBoost, 음수면 Slowness 적용. 0이면 아무것도 적용하지 않음.
     * 4-4. 공격력 30% 감소
     * 4-5. 공격 시 피해량의 8%를 회복함(Owner.Heal 로 적용.)
     *
     * 5. 환멸의 형상 능력치
     * 5-1. 1초 간격으로 최대 HP의 2%만큼 자가 피해를 입으며, 최소치는 최대 체력의 10%까지만 감소 가능.
     * 5-2. 이동 속도 45% 증가.(AddEffect로 추가)
     * 5-3. 공격력 45% 증가.
     * 5-4. 공격 시, 자신의 잃은 HP만큼 추가 피해를 입히며, 해당 피해는 관통 고정 피해로 적용(ApplyFixedDamage)
     * 5-5. 
     */
}