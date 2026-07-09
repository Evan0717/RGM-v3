using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost4;

[Echo("살라만드라", "사용 시 자신이 공격하는 모든 공격에 화상 효과 적용. 지속 시간 30초, 재사용 대기시간 60초", EchoType.Salamandra, EchoCost.Cost4, EchoMainStatType.AttackPercent, "🔥")]
public class Salamandra : EchoActiveAbility
{
    public override float Duration => 30f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "30초간 공격에 화상 부여";

    bool _active;

    public override void OnEnabled()
    {
        base.OnEnabled();
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        _active = false;
        base.OnDisabled();
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

        ev.Player.EnableEffect(EffectType.Burned, 1, 3f);
    }
}