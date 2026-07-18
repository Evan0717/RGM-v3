using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Scp049;
using MEC;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost3;

[Echo("쁘띠 049", "SCP-049가 사용 시, 스킬 사용 후 다음 소생한 049-2의 체력이 50% 증가. 재사용 대기시간 60초", EchoType.Chibi049, EchoCost.Cost3, EchoMainStatType.SizeReduction, "🐶")]
public class Chibi049 : EchoActiveAbility
{
    private bool _armed;

    public override float Duration => 0f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "소생한 049-2의 체력이 50% 증가.";

    protected override bool CanUseActive() =>
        base.CanUseActive() && Owner.Role is Scp049Role && !_armed;

    protected override void OnActiveUsed()
    {
        _armed = true;
        Exiled.Events.Handlers.Scp049.FinishingRecall += OnFinishingRecall;
    }

    public override void OnActiveEffect()
    {
        Disarm();
        base.OnActiveEffect();
    }

    private void OnFinishingRecall(FinishingRecallEventArgs ev)
    {
        if (!_armed || ev.Player != Owner)
            return;

        Disarm();
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (ev.Target == null || !ev.Target.IsAlive)
                return;

            ev.Target.MaxHealth *= 1.5f;
            ev.Target.Health = ev.Target.MaxHealth;
        });
    }

    private void Disarm()
    {
        _armed = false;
        Exiled.Events.Handlers.Scp049.FinishingRecall -= OnFinishingRecall;
    }
}