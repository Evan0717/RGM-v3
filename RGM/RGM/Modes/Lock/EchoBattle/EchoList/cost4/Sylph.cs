using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost4;

[Echo("실프", "12초간 이동 속도 50% 증가 및 Ghostly, Fade 효과 적용, 재사용 대기시간 60초", EchoType.Sylph, EchoCost.Cost4, EchoMainStatType.MoveSpeedAndJump, "🌬️")]
public class Sylph : EchoActiveAbility
{
    public override float Duration => 12f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "12초간 이속 50% + Ghostly/Fade";

    protected override void OnActiveUsed()
    {
        Owner.AddEffect(EffectType.MovementBoost, 50, Duration);
        Owner.EnableEffect(EffectType.Ghostly, 1, Duration);
        Owner.EnableEffect(EffectType.Fade, 64, Duration);
    }
}