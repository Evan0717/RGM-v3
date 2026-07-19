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
/// Passive: HP 16%+(res*4%) and critical chance 5%. On lethal hit: invuln/invis/speed for (1.2s*res), max 3 times.
/// On trigger: heal MaxHP * (10% + 5%*res).
/// </summary>
[ExclusiveWeapon(
    "피안화",
    "HP 16% + (공진 수치 * 4%) 및 크리티컬 확률 5% 증가. 사망에 이르는 피해 시 (1.2초 * 공진 수치)간 무적·투명·이속 증가(최대 3회). 발동 시 최대 체력의 15% + (5% * 공진 수치) 회복.",
    ExclusiveWeaponType.DeathFlower)]
public class DeathFlower : ExcWeapon
{
    public override float AttackFlatMin => 2.5f;
    public override float AttackFlatMax => 31.3f;
    public override ExclusiveWeaponSecondaryStat SecondaryStat => ExclusiveWeaponSecondaryStat.HpPercent;
    public override float SecondaryStatMin => 13.2f;
    public override float SecondaryStatMax => 59.6f;

    public override float PassiveHpPercent => 16f + Resonance * 4f;
    public override float PassiveCriticalChance => 5f;

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

        float duration = 1.2f * Resonance;
        float speedBonus = 16f + 4f * Resonance;
        float heal = Owner.MaxHealth * (0.10f + 0.05f * Resonance);

        float room = Owner.MaxHealth - Owner.Health;
        if (room > 0f)
            // Heal()은 Healing 이벤트를 발생시켜 치료 효과 보너스가 적용된다.
            Owner.Health += Mathf.Min(heal, room);

        _invulnerable = true;
        Owner.EnableEffect(EffectType.Invisible, 1, duration);
        Owner.EnableEffect(EffectType.Ghostly, 1, duration);
        Owner.EnableEffect(EffectType.MovementBoost, (byte)Mathf.Clamp(Mathf.RoundToInt(speedBonus), 1, 255), duration);

        Timing.KillCoroutines($"DeathFlower_{Owner.UserId}");
        Timing.RunCoroutine(InvulnerabilityRoutine(duration), $"DeathFlower_{Owner.UserId}");

        EchoBattleCore.ShowNotification(
            Owner,
            $"<color=#ff6699>피안화</color> 발동 ({_triggersUsed}/{MaxTriggers}) · {duration:0.#}초 무적",
            2f);
    }

    IEnumerator<float> InvulnerabilityRoutine(float duration)
    {
        yield return Timing.WaitForSeconds(duration);
        _invulnerable = false;
    }
}
