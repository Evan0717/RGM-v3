using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace RGM.Modes.Abilities.Ancient;

[Ability(
    "ANTI MATTER",
    """
    자신의 모든 공격에 폭발을 일으킵니다. 해당 피해는 적의 방어 효과를 무시합니다.
    추가로, 받는 데미지가 1로 고정되며, 해당 효과는 방어 무시 능력의 영향을 받지 않습니다.
    """,
    AbilityCategory.Ancient,
    AbilityType.ANCIENT_EXPLOSIVEAMMO)]

public class AntiMatter : Ability
{
    private const float ExplosionInterval = 0.1f;
    private const float FixedDamage = 1f;

    private float _nextExplosionTime;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        
        Timing.CallDelayed(0.5f, () =>
            {
                Owner.AddAbility(AbilityType.SYNERGY_WEAKPOINTATTACK);
                Owner.AddAbility(AbilityType.EPIC_HOLYPROTECTION);
                Owner.AddAbility(AbilityType.RARE_SAVELOCATION);
            });
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player == Owner && ev.IsAllowed)
            FixIncomingDamage(ev);

        if (ev.Attacker != Owner ||
            !ev.IsAllowed ||
            ev.DamageHandler.Damage <= 0f ||
            ev.DamageHandler.Type == DamageType.Explosion ||
            !HitboxIdentity.IsEnemy(Owner.ReferenceHub, ev.Player.ReferenceHub) ||
            Time.time < _nextExplosionTime)
        {
            return;
        }

        _nextExplosionTime = Time.time + ExplosionInterval;

        var grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Owner);
        grenade.FuseTime = 0.01f;
        grenade.SpawnActive(ev.Player.Position, Owner);
    }

    private static void FixIncomingDamage(HurtingEventArgs ev)
    {
        if (ev.IsInstantKill)
        {
            ev.IsAllowed = false;
            ev.Player.Hurt(FixedDamage, ev.DamageHandler.Type);
            return;
        }

        ev.DamageHandler.Damage = FixedDamage;
    }
}