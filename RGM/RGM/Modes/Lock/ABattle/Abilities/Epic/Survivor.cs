using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp106;
using Exiled.Events.EventArgs.Warhead;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Epic;

[Ability("구사일생", "사망 판정을 받을 경우, 2초간 투명 상태와 무적이 되며, 체력을 20% 회복합니다. (최대 3번)", AbilityCategory.Epic, AbilityType.EPIC_SURVIVOR)]
public class Survivor : Ability
{
    private const float InvincibilityDuration = 2f;

    private static bool _isDetonatingState;

    private int power = 3;
    private bool isEnabled;
    private int _version;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Scp106.Attacking += OnScp106Attacking;
        Exiled.Events.Handlers.Warhead.Detonating += OnDetonating;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Exiled.Events.Handlers.Scp106.Attacking -= OnScp106Attacking;
        Exiled.Events.Handlers.Warhead.Detonating -= OnDetonating;

        _version++;
        isEnabled = false;
    }

    private void OnDetonating(DetonatingEventArgs _)
    {
        if (_isDetonatingState)
            return;

        _isDetonatingState = true;
        Timing.CallDelayed(Timing.WaitForOneFrame, () => _isDetonatingState = false);
    }

    private void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner || IsExemptDamage(ev.DamageHandler.Type))
            return;

        if (isEnabled)
        {
            ev.IsAllowed = false;
            return;
        }

        if (TrySurvive())
            ev.IsAllowed = false;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player == Owner &&
            !isEnabled &&
            !IsExemptDamage(ev.DamageHandler.Type) &&
            IsLethalDamage(ev) &&
            TrySurvive())
        {
            ev.IsAllowed = false;
            ev.DamageHandler.Damage = 0f;
            return;
        }

        if (isEnabled &&
            ev.Player == Owner &&
            (!IsExemptDamage(ev.DamageHandler.Type) ||
             ev.DamageHandler.Type == DamageType.PocketDimension && !ev.IsInstantKill))
            ev.IsAllowed = false;
    }

    private void OnScp106Attacking(AttackingEventArgs ev)
    {
        if (ev.Target != Owner || !isEnabled)
            return;

        ev.IsAllowed = false;
    }

    private bool TrySurvive()
    {
        if (!ABattle.Instance.IsLifeUsed.TryGetValue(Owner, out bool isLifeUsed))
            ABattle.Instance.IsLifeUsed[Owner] = false;
        else if (isLifeUsed)
            return false;

        ABattle.Instance.IsLifeUsed[Owner] = true;
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (ABattle.Instance.IsLifeUsed.ContainsKey(Owner))
                ABattle.Instance.IsLifeUsed[Owner] = false;
        });

        ActivateSurvivor();
        return true;
    }

    private bool IsLethalDamage(HurtingEventArgs ev)
    {
        float damage = ev.DamageHandler.Damage;
        if (damage <= 0f && !ev.IsInstantKill)
            return false;

        float totalHealth = Owner.Health + Owner.ArtificialHealth + Owner.HumeShield;
        return ev.IsInstantKill || damage >= totalHealth;
    }

    private void ActivateSurvivor()
    {
        isEnabled = true;

        Owner.EnableEffect(EffectType.Invisible, 1, InvincibilityDuration);
        Owner.EnableEffect(EffectType.Ghostly, 1, InvincibilityDuration);
        Owner.AddEffect(EffectType.MovementBoost, 20, InvincibilityDuration);
        Owner.Heal(Owner.MaxHealth * 0.2f);

        int remaining = power - 1;
        int version = ++_version;
        bool removeAfter = power == 1;
        if (!removeAfter)
            power--;

        Timing.CallDelayed(InvincibilityDuration, () =>
        {
            if (_version != version)
                return;

            isEnabled = false;

            if (removeAfter)
                Owner.RemoveAbility(this);
        });

        Owner.AddHint("구사일생", $"<color={ABattle.RatingColor["영웅"]}>구사일생</color> 능력으로 인해 3초간 죽음을 피합니다. ({remaining}번 남음)");
    }

    private static bool IsExemptDamage(DamageType damageType)
    {
        return _isDetonatingState ||
               damageType is DamageType.Warhead or DamageType.PocketDimension or DamageType.Crushed;
    }
}
