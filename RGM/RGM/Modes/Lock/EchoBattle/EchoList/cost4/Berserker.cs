using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost4;

[Echo("광전사", "사용 시 2초간 자신의 최대 체력의 10%만큼 추가 데미지, 재사용 대기시간 60초", EchoType.Berserker, EchoCost.Cost4, EchoMainStatType.AttackPercent, "⚔️")]
public class Berserker : EchoActiveAbility
{
    public override float Duration => 2f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "2초간 자신의 최대 HP 10% 추가 데미지";

    bool _active;

    public override void OnEnabled()
    {
        base.OnEnabled();
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void ONActiveEffect()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        _active = false;
        base.ONActiveEffect();
    }

    protected override void OnActiveUsed()
    {
        _active = true;
        Timing.CallDelayed(Duration, () => _active = false);
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (!_active || ev.Attacker != Owner)
            return;

        if (!HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
            return;

        if (EchoStats.AreAttackModifiersIgnored(Owner))
            return;

        ev.DamageHandler.Damage += Owner.MaxHealth * 0.1f;

        Timing.CallDelayed(Timing.WaitForOneFrame, () => ev.Attacker.ShowHitMarker(2));
    }
}