using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Epic;

[Ability("수어사이드 봄버맨", """
                      사망할 경우 즉시 폭발합니다. 51% 확률로 연쇄 폭발이 일어납니다.
                      연쇄 폭발 시 각 연쇄 마다 폭발 데미지가 20%씩 감소합니다.
                      """, AbilityCategory.Epic, AbilityType.EPIC_SUICIDEBOMBER)]
public class SuicideBomber : Ability
{
    private const float ChainDamageMultiplier = 0.8f;
    private const float ExplosionInterval = 0.2f;

    private CoroutineHandle _chainExplosionCoroutine;
    private bool _isChainExplosionActive;
    private float _explosionDamageMultiplier = 1f;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Timing.KillCoroutines(_chainExplosionCoroutine);
        _isChainExplosionActive = false;
        _explosionDamageMultiplier = 1f;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (!_isChainExplosionActive ||
            ev.Attacker != Owner ||
            ev.DamageHandler.Type != DamageType.Explosion)
            return;

        ev.DamageHandler.Damage *= _explosionDamageMultiplier;
    }

    private void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        Vector3 pos = Owner.Position;

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (!Owner.IsDead) return;

            _chainExplosionCoroutine = Timing.RunCoroutine(ExplodeInChain(pos));
        });
    }

    private IEnumerator<float> ExplodeInChain(Vector3 position)
    {
        _isChainExplosionActive = true;
        _explosionDamageMultiplier = 1f;

        do
        {
            var grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Owner);
            grenade.FuseTime = 0.1f;
            grenade.SpawnActive(position, Owner);

            yield return Timing.WaitForSeconds(ExplosionInterval);
            _explosionDamageMultiplier *= ChainDamageMultiplier;
        }
        while (Random.Range(1, 101) <= 51);

        _isChainExplosionActive = false;
        _explosionDamageMultiplier = 1f;

        if (GodModePlayers.Contains(Owner)) GodModePlayers.Remove(Owner);
    }
}