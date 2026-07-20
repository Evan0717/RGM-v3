using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Scp173;
using MEC;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost3;

[Echo("쁘띠 173", "SCP-173이 사용 시 다음 텔레포트 시간 1초로 설정, 재사용 대기시간 60초", EchoType.Chibi173, EchoCost.Cost3, EchoMainStatType.HpPercent, "🐶")]
public class Chibi173 : EchoActiveAbility
{
    private Scp173Role _scp173;
    private float _originalCooldown;

    public override float Duration => 0f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "다음 텔레포트 시간 1초로 설정";

    protected override bool CanUseActive() =>
        base.CanUseActive() && Owner.Role is Scp173Role;

    protected override void OnActiveUsed()
    {
        _scp173 = Owner.Role as Scp173Role;
        if (_scp173 == null)
            return;

        _originalCooldown = _scp173.BlinkCooldown;
        _scp173.BlinkCooldown = 1f;
        Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;
    }

    public override void OnActiveEffect()
    {
        RestoreCooldown();
        base.OnActiveEffect();
    }

    private void OnBlinking(BlinkingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;
        Timing.CallDelayed(Timing.WaitForOneFrame, RestoreCooldown);
    }

    private void RestoreCooldown()
    {
        Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;

        if (_scp173 != null)
            _scp173.BlinkCooldown = _originalCooldown;

        _scp173 = null;
    }
}