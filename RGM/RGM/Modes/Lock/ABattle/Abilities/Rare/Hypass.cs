using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp106;
using MEC;

namespace RGM.Modes.Abilities.Rare;

[Ability("하이패스", "15초 간 모든 공격에 무적이 됩니다.", AbilityCategory.Rare, AbilityType.RARE_HYPASS)]
public class Hypass : Ability
{
    private const float Duration = 15f;

    private bool _isActive;
    private int _version;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Scp106.Attacking += OnScp106Attacking;

        _isActive = true;
        int version = ++_version;

        Timing.CallDelayed(Duration, () =>
        {
            if (_isActive && _version == version)
                EndInvincibility();
        });
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Exiled.Events.Handlers.Scp106.Attacking -= OnScp106Attacking;

        _version++;
        _isActive = false;
    }

    private void EndInvincibility()
    {
        if (!_isActive)
            return;

        _isActive = false;
        _version++;
    }

    private void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner || !_isActive || IsExemptDamage(ev.DamageHandler.Type))
            return;

        ev.IsAllowed = false;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (_isActive &&
            ev.Player == Owner &&
            (!IsExemptDamage(ev.DamageHandler.Type) ||
             ev.DamageHandler.Type == DamageType.PocketDimension && !ev.IsInstantKill))
            ev.IsAllowed = false;
    }

    private void OnScp106Attacking(AttackingEventArgs ev)
    {
        if (ev.Target != Owner || !_isActive)
            return;

        ev.IsAllowed = false;
    }

    private static bool IsExemptDamage(DamageType damageType)
    {
        return damageType is DamageType.Warhead or DamageType.PocketDimension or DamageType.Crushed;
    }
}
