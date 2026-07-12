using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using RGM.Modes;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost3;

[Echo("쁘띠 096", "SCP-096이 사용 시 즉시 폭주 및 주변 20m 대상 전체를 목격자로 포함, 8초간 받는 데미지 25% 감소, 재사용 대기시간 90초", EchoType.Chibi096, EchoCost.Cost3, EchoMainStatType.AttackPercent, "🐶")]
public class Chibi096 : EchoActiveAbility
{
    private bool _damageReductionActive;

    public override float Duration => 8f;
    public override float Cooldown => 90f;
    public override string ActiveDescription => "즉시 폭주 및 주변 20m 대상 전체를 목격자로 포함, 8초간 받는 데미지 25% 감소";

    protected override bool CanUseActive() =>
        base.CanUseActive()
        && Owner.Role is Scp096Role scp096
        && !scp096.RageManager.IsEnraged;

    protected override void OnActiveUsed()
    {
        if (Owner.Role is not Scp096Role scp096)
            return;

        foreach (var target in PlayerManager.List.Where(player =>
                     player.IsAlive
                     && player.IsHuman
                     && Vector3.Distance(player.Position, Owner.Position) <= 20f))
        {
            scp096.AddTarget(target);
        }

        _damageReductionActive = true;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Timing.CallDelayed(Duration, DisableDamageReduction);

        scp096.Enrage(Duration);
    }

    public override void ONActiveEffect()
    {
        DisableDamageReduction();
        base.ONActiveEffect();
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (_damageReductionActive
            && ev.Player == Owner
            && !EchoStats.IsApplyingFixedDamage(ev.Player)
            && ev.DamageHandler?.Type != DamageType.Crushed)
        {
            ev.Amount *= 0.75f;
        }
    }

    private void DisableDamageReduction()
    {
        _damageReductionActive = false;
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }
}