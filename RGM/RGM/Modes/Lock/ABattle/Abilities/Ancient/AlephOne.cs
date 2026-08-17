using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Ancient;

[Ability(
    "ALEPH-1", 
    """
    <color=#FF3131>모든 것을 허무로 되돌려버립니다.</color>
    획득 시, 자신과 아군을 제외한 모든 상대의 체력을 1로 상시 고정하고, 감소된 수치만큼 본인의 최대 HP가 상승합니다.
    추가로, 자신의 최대 HP의 40%만큼 추가 피해를 입히며, 본인은 받는 피해가 최대 10까지 적용됩니다.
    """,
    AbilityCategory.Ancient,
    AbilityType.ANCIENT_ALEPHONE)] 
public class AlephOne : Ability
{
    private const float EnemyHealth = 1f;
    private const float AdditionalDamageRatio = 0.4f;
    private const float MaximumIncomingDamage = 10f;

    private CoroutineHandle _healthLockCoroutine;
    private readonly HashSet<Player> _processedEnemies = [];

    public override void OnEnabled()
    {        
        _processedEnemies.Clear();

        foreach (Player enemy in PlayerManager.List.Where(IsEnemy))
            ApplyHealthLock(enemy);

        Exiled.Events.Handlers.Player.Healing += OnHealing;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Player.Died += OnDied;
        _healthLockCoroutine = Timing.RunCoroutine(LockEnemyHealth());
        
        Timing.CallDelayed(0.5f, () =>
        {
            Owner.AddAbility(AbilityType.SYNERGY_WEAKPOINTATTACK);
            Owner.AddAbility(AbilityType.EPIC_HOLYPROTECTION);
        });
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Healing -= OnHealing;
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Exiled.Events.Handlers.Player.Died -= OnDied;
        Timing.KillCoroutines(_healthLockCoroutine);
        _processedEnemies.Clear();
    }

    private bool IsEnemy(Player player) =>
        player != Owner &&
        player.IsAlive &&
        HitboxIdentity.IsEnemy(Owner.ReferenceHub, player.ReferenceHub);

    private static void LockHealth(Player player)
    {
        player.Health = EnemyHealth;
        player.ArtificialHealth = 0f;
        player.HumeShield = 0f;
    }

    private IEnumerator<float> LockEnemyHealth()
    {
        while (true)
        {
            foreach (Player enemy in PlayerManager.List.Where(IsEnemy))
                ApplyHealthLock(enemy);

            yield return Timing.WaitForSeconds(0.1f);
        }
    }

    private void ApplyHealthLock(Player enemy)
    {
        if (_processedEnemies.Add(enemy))
        {
            float reducedHealth = enemy.Health - EnemyHealth;
            if (reducedHealth > 0f)
            {
                Owner.MaxHealth += reducedHealth;
                Owner.Health += reducedHealth;
            }
        }

        LockHealth(enemy);
    }

    private void OnHealing(HealingEventArgs ev)
    {
        if (IsEnemy(ev.Player))
            ev.Amount = 0f;
    }

    private void OnDied(DiedEventArgs ev)
    {
        _processedEnemies.Remove(ev.Player);
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player == Owner && ev.IsAllowed)
        {
            if (ev.IsInstantKill)
            {
                ev.IsAllowed = false;
                Owner.Hurt(MaximumIncomingDamage, ev.DamageHandler.Type);
                return;
            }

            if (ev.DamageHandler.Damage > MaximumIncomingDamage)
                ev.DamageHandler.Damage = MaximumIncomingDamage;
        }

        if (ev.Attacker == Owner && IsEnemy(ev.Player))
            ev.DamageHandler.Damage += Owner.MaxHealth * AdditionalDamageRatio;
    }
}