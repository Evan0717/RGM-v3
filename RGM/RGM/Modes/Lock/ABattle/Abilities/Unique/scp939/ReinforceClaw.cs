using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using System;

namespace RGM.Modes.Abilities.Unique.Scp939;

[Ability("발톱 강화", "발톱 공격으로 연속 공격 시 데미지가 15씩 증가합니다.\n3초 이내에 타격이 없을 경우 수치가 초기화됩니다.", AbilityCategory.Scp939, AbilityType.SCP939_REINFORCECLAW)]
public class ReinforceClaw : Ability
{
    private const float DamageIncrease = 15f;
    private static readonly TimeSpan ResetTime = TimeSpan.FromSeconds(3);
    
    private float _damageBonus;
    private DateTime _lastClawHitTime = DateTime.MinValue;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        _damageBonus = 0f;
        _lastClawHitTime = DateTime.MinValue;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (Owner.Role is not Scp939Role scp939) return;
        if (ev.Attacker == null || ev.Attacker.ReferenceHub != scp939.Owner.ReferenceHub) return;
        if (ev.DamageHandler.Type != DamageType.Scp939) return;

        DateTime now = DateTime.UtcNow;

        if (now - _lastClawHitTime > ResetTime)
            _damageBonus = 0f;

        ev.DamageHandler.Damage += _damageBonus;

        _damageBonus += DamageIncrease;
        _lastClawHitTime = now;
    }
}
