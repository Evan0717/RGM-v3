using Exiled.Events.EventArgs.Player;
using RGM.Modes;
using UnityEngine;

namespace RGM.Modes.ExclusiveWeapon;

/// <summary>
/// KumoKiri.
/// Passive: Attack 11%+(res*2%). On hit (1%*res) chance for 618.03 fixed damage.
/// </summary>
[ExclusiveWeapon(
    "쿠모키리",
    "공격력 11% + (공진 수치 * 2%) 증가. 적 타격 시 공진 수치 * (공진 수치 + 1) / 2 확률로 618.03 * (0.5 + 0.5 * 공진 수치) 만큼 고정 피해.",
    ExclusiveWeaponType.KumoKiri)]
public class KumoKiri : ExcWeapon
{
    public override float AttackFlatMin => 2.5f;
    public override float AttackFlatMax => 31.3f;
    public override ExclusiveWeaponSecondaryStat SecondaryStat => ExclusiveWeaponSecondaryStat.CriticalChance;
    public override float SecondaryStatMin => 13.0f;
    public override float SecondaryStatMax => 41.0f;

    public override float PassiveAttackPercent => 11f + Resonance * 2f;

    const float FixedDamage = 618.03f;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != Owner || Owner == null)
            return;

        if (EchoStats.IsApplyingFixedDamage(ev.Player))
            return;

        if (ev.DamageHandler == null || ev.DamageHandler.Damage <= 0f)
            return;

        if (!HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        if (EchoStats.AreAttackModifiersIgnored(Owner))
            return;

        float chance = Resonance * (Resonance + 1) / 2f;
        if (Random.Range(0f, 100f) >= chance)
            return;

        EchoStats.DealFixedDamage(ev.Player, Owner, FixedDamage * (0.5f + 0.5f * Resonance));
        EchoBattleCore.ShowNotification(
            Owner,
            $"<color=#cc88ff>쿠모키리</color> 고정 피해 +{FixedDamage * (0.5f + 0.5f * Resonance):0.##}",
            1.2f);
    }
}
