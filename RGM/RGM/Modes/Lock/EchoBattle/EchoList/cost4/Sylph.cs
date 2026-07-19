using Exiled.API.Enums;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost4;

[Echo("실프", "10초간 이동 속도 40% 증가 및 문 통과/반투명화/발소리 제거 효과 적용. 재사용 대기시간 60초", EchoType.Sylph, EchoCost.Cost4, EchoMainStatType.MoveSpeedAndJump, "🌬️")]
public class Sylph : EchoActiveAbility
{
    public override float Duration => 10f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "10초간 이속 40% + 문 통과/반투명화/발소리 제거";

    protected override void OnActiveUsed()
    {
        Owner.AddEffect(EffectType.MovementBoost, 40, Duration);
        Owner.EnableEffect(EffectType.Ghostly, 1, Duration);
        Owner.EnableEffect(EffectType.Fade, 64, Duration);
        Owner.EnableEffect(EffectType.SilentWalk, 11, Duration);
    }
}