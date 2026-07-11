using Exiled.API.Enums;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost4;

[Echo("실프", "10초간 이동 속도 80% 증가 및 Ghostly, Fade 효과 적용. 재사용 대기시간 60초", EchoType.Sylph, EchoCost.Cost4, EchoMainStatType.MoveSpeedAndJump, "🌬️")]
public class Sylph : EchoActiveAbility
{
    public override float Duration => 10f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "12초간 이속 80% + Ghostly/Fade";

    protected override void OnActiveUsed()
    {
        Owner.AddEffect(EffectType.MovementBoost, 80, Duration);
        Owner.EnableEffect(EffectType.Ghostly, 1, Duration);
        Owner.EnableEffect(EffectType.Fade, 64, Duration);
    }
}