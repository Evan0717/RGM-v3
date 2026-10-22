using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using RGM.API.Features;
using UnityEngine;
using Utils.Networking;

namespace RGM.Modes.Abilities.Mythic;

[Ability("철퇴 자크", """
                  착지하는 순간 일대가 초토화됩니다.
                  착지 지점 반경 10m 이내의 모든 적은 『죽음에 이르는 공격』을 받습니다.
                  """, 
    AbilityCategory.Mythic, AbilityType.MYTHIC_HALLOWEEN_HAMMER, 
    RoleAbility.None, false, AbilityHolidayType.Halloween)]
public class Hammer : Ability
{
    private const float AttackRadius = 10f;
    private const float FallDamage = 1f;
    private const float MetalDamageMultiplier = 0.2f;
    private const float DamageReductionMultiplier = 0.6f;

    public override void OnEnabled()
    {
        Owner.AddAbility(AbilityType.LEGEND_SPEEDWAGON);
        Owner.AddEffect(EffectType.Metal, 255);
        Owner.AddEffect(EffectType.Lightweight, 255);
        Owner.AddEffect(EffectType.DamageReduction, 80);

        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner ||
            ev.DamageHandler.Type != DamageType.Falldown ||
            !Owner.TryGetEffect(EffectType.Metal, out StatusEffectBase metal) ||
            !metal.IsEnabled)
        {
            return;
        }

        // Metal applies its damage modifier after the Hurting event. Reducing
        // the original fall hit to 1 here makes Metal reduce it to 0, which
        // suppresses the native fall-sound message. Cancel that hit, send its
        // custom Metal sound explicitly, then apply a separate final 1 damage.
        ev.IsAllowed = false;
        NetworkUtils.SendToAuthenticated(
            new FpcFallDamageMessage(Owner.ReferenceHub, Owner.Position, Owner.Role.Type),
            0);
        Owner.Hurt(
            FallDamage / (MetalDamageMultiplier * DamageReductionMultiplier),
            DamageType.Custom);

        foreach (var target in PlayerManager.List.Where(player =>
                     player.IsAlive &&
                     HitboxIdentity.IsEnemy(Owner.ReferenceHub, player.ReferenceHub) &&
                     Vector3.Distance(player.Position, Owner.Position) <= AttackRadius))
        {
            if (ApplyLethalDamage.Apply(Owner, target)) Owner.ShowHitMarker(1.4f);
        }
    }
}