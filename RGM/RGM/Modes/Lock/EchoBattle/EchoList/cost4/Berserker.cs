using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost4;

[Echo("광전사", "사용 시 다음 공격은 자신의 최대 체력의 60%만큼 추가 데미지, 재사용 대기시간 60초", EchoType.Berserker, EchoCost.Cost4, EchoMainStatType.AttackPercent, "⚔️")]
public class Berserker : EchoActiveAbility
{
    public override float Duration => 0f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "다음 공격에 최대 HP 60% 추가 데미지";

    bool _armed;

    public override void OnEnabled()
    {
        base.OnEnabled();
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        _armed = false;
        base.OnDisabled();
    }

    protected override void OnActiveUsed()
    {
        _armed = true;
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (!_armed || ev.Attacker != Owner)
            return;

        if (!HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        ev.DamageHandler.Damage += Owner.MaxHealth * 0.6f;
        _armed = false;

        Timing.CallDelayed(Timing.WaitForOneFrame, () => ev.Attacker.ShowHitMarker(2));
    }
}