using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Ancient;

[Ability(
    "ALEPH-1", 
    """
    <color=#FF3131>모든 것을 허무로 되돌려버립니다.</color>
    획득 시, 자신과 아군을 제외한 모든 상대의 체력을 1로 상시 고정하고, 감소된 수치만큼 본인의 최대 HP가 상승합니다.
    추가로, 자신의 최대 HP의 30%만큼 추가 피해를 입히며, 본인은 『피격 제한』이 최대 HP의 0.1%까지 적용됩니다.
    ALEPH-1은 1명만 존재할 수 있으며, 능력 획득 시 이전 ALEPH-1의 HP를 흡수한 뒤 모든 능력을 제거하고 관전자로 전환합니다.
    """,
    AbilityCategory.Ancient,
    AbilityType.ANCIENT_ALEPHONE)] 
public class AlephOne : Ability
{
    private const float EnemyHealth = 1f;
    private const float AdditionalDamageRatio = 0.3f;
    private const float MaxHealthRatio = 0.001f;

    private CoroutineHandle _healthLockCoroutine;
    private readonly HashSet<Player> _processedEnemies = [];

    public override void OnEnabled()
    {        
        _processedEnemies.Clear();
        AbsorbPreviousAlephOnes();

        foreach (Player enemy in PlayerManager.List.Where(IsEnemy))
            ApplyHealthLock(enemy);

        Exiled.Events.Handlers.Player.Healing += OnHealing;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Player.Died += OnDied;
        _healthLockCoroutine = Timing.RunCoroutine(LockEnemyHealth());
        
        Timing.CallDelayed(0.5f, () =>
        {
            Owner.AddAbility(AbilityType.EPIC_SHARPEYES);
            Owner.AddAbility(AbilityType.EPIC_SHARPEYES);
            Owner.AddAbility(AbilityType.RARE_BULLSEYE);
            Owner.AddAbility(AbilityType.EPIC_HOLYPROTECTION);
            Owner.AddAbility(AbilityType.RARE_SAVELOCATION);
        });
    }

    private void AbsorbPreviousAlephOnes()
    {
        foreach (Player previousAlephOne in PlayerManager.List
                     .Where(player => player != Owner && player.HasAbility(AbilityType.ANCIENT_ALEPHONE))
                     .ToList())
        {
            float absorbedHealth = previousAlephOne.Health;
            Owner.MaxHealth += absorbedHealth;
            Owner.Health += absorbedHealth;

            previousAlephOne.RemoveAllAbilities();
            previousAlephOne.Role.Set(RoleTypeId.Spectator);
        }
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
        player.HumeShield = 1f;
    }

    private IEnumerator<float> LockEnemyHealth()
    {
        while (true)
        {
            foreach (Player enemy in PlayerManager.List.Where(IsEnemy))
                ApplyHealthLock(enemy);

            yield return Timing.WaitForSeconds(0.5f);
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
                Owner.Hurt(Owner.MaxHealth * MaxHealthRatio, ev.DamageHandler.Type);
                return;
            }

            if (ev.DamageHandler.Damage > Owner.MaxHealth * MaxHealthRatio)
                ev.DamageHandler.Damage = Owner.MaxHealth * MaxHealthRatio;
        }

        if (ev.Attacker == Owner && IsEnemy(ev.Player))
            ev.DamageHandler.Damage += Owner.MaxHealth * AdditionalDamageRatio;
    }
}