using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Legend;

[Ability("리인카네이션", "사망 판정을 받을 시, 60초간 모든 공격에 무적이 되며, 능력치가 상승합니다.\n단, 일정 횟수 이상 적 타격에 실패할 시 사망합니다.", AbilityCategory.Legend, AbilityType.LEGEND_REINCARNATION)]
public class Reincarnation : Ability
{
    private const float ContractDuration = 60f;
    private const float CooldownDuration = 180f;
    private const int RequiredHits = 90;
    private const int RequiredKills = 7;
    private const int MovementBoostIntensity = 30;

    private bool _isContractActive;
    private bool _isCoolingDown;
    private int _hitCount;
    private int _killCount;
    private int _contractVersion;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Player.Hurt += OnHurt;
        Exiled.Events.Handlers.Player.Died += OnDied;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Exiled.Events.Handlers.Player.Hurt -= OnHurt;
        Exiled.Events.Handlers.Player.Died -= OnDied;

        _contractVersion++;

        if (_isContractActive)
            Owner.RemoveEffect(EffectType.MovementBoost, MovementBoostIntensity);

        _isContractActive = false;
        _isCoolingDown = false;
        _hitCount = 0;
        _killCount = 0;
    }

    private void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner || IsContractExemptDamage(ev.DamageHandler.Type))
            return;

        if (_isContractActive)
        {
            ev.IsAllowed = false;
            return;
        }

        if (_isCoolingDown || ABattle.Instance.IsLifeUsed[Owner])
            return;

        ev.IsAllowed = false;
        ABattle.Instance.IsLifeUsed[Owner] = true;
        Timing.CallDelayed(Timing.WaitForOneFrame, () => ABattle.Instance.IsLifeUsed[Owner] = false);

        StartContract();
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (_isContractActive && ev.Player == Owner && !IsContractExemptDamage(ev.DamageHandler.Type))
            ev.IsAllowed = false;

        if (_isContractActive && ev.Attacker == Owner)
            ev.DamageHandler.Damage *= 1.5f;
    }

    private void OnHurt(HurtEventArgs ev)
    {
        if (!_isContractActive ||
            ev.Attacker == null ||
            ev.Attacker != Owner ||
            ev.DamageHandler.Damage <= 0 ||
            !HitboxIdentity.IsEnemy(Owner.ReferenceHub, ev.Player.ReferenceHub))
            return;

        _hitCount++;

        if (_hitCount >= RequiredHits)
            EndContract(success: true);
    }

    private void OnDied(DiedEventArgs ev)
    {
        if (!_isContractActive ||
            ev.Attacker != Owner ||
            !HitboxIdentity.IsEnemy(Owner.ReferenceHub, ev.Player.ReferenceHub))
            return;

        _killCount++;

        if (_killCount >= RequiredKills)
            EndContract(success: true);
    }

    private void StartContract()
    {
        _isContractActive = true;
        _hitCount = 0;
        _killCount = 0;

        foreach (var effectType in Owner.ActiveEffects
                     .Select(effect => effect.GetEffectType())
                     .Where(effectType => !EffectManager.IsKeptBuff(effectType))
                     .ToList())
        {
            Owner.DisableEffect(effectType);
        }

        Owner.AddEffect(EffectType.MovementBoost, MovementBoostIntensity, ContractDuration);
        Owner.AddHint("리인카네이션",
            $"영혼 계약이 시작되었습니다. {ContractDuration:0}초 내에 적을 {RequiredHits}회 타격하거나 {RequiredKills}회 처치하세요.");

        int contractVersion = ++_contractVersion;
        Timing.CallDelayed(ContractDuration, () =>
        {
            if (_isContractActive && _contractVersion == contractVersion)
                EndContract(success: false);
        });
    }

    private void EndContract(bool success)
    {
        if (!_isContractActive)
            return;

        _isContractActive = false;
        _contractVersion++;
        Owner.RemoveEffect(EffectType.MovementBoost, MovementBoostIntensity);

        if (!success)
        {
            _isCoolingDown = true;
            Owner.Kill("영혼 계약에 실패하여 산화되었습니다.");
            return;
        }

        Owner.Health = Owner.MaxHealth;
        _isCoolingDown = true;
        Owner.AddHint("리인카네이션",
            $"영혼 계약을 달성했습니다. {CooldownDuration:0}초 후 다시 사용할 수 있습니다.");

        Timing.CallDelayed(CooldownDuration, () => _isCoolingDown = false);
    }

    private static bool IsContractExemptDamage(DamageType damageType)
    {
        return damageType is DamageType.Warhead or DamageType.PocketDimension or DamageType.Crushed;
    }
}