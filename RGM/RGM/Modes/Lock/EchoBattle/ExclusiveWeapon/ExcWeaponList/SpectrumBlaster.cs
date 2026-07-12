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
    "스펙트럼 블래스터",
    "공격력 8% + (공진 수치 * 2%) 증가. 적에게 10회 타격을 입힐 때마다 다음 공격은 관통력이 (10% * 공진 수치)만큼 증가.",
    ExclusiveWeaponType.SpectrumBlaster)]
public class SpectrumBlaster : ExcWeapon
{
    public override float AttackFlatMin => 4.7f;
    public override float AttackFlatMax => 57.6f;
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

        if (EchoStats.AreAttackModifiersIgnored(Owner))
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
            EchoBattleCore.ShowNotification(
                Owner,
                $"<color=#88aaff>스펙트럼</color> 다음 공격 관통 +{10 * Resonance}%",
                1.5f);
        }
    }
}
