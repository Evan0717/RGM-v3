using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost3;

[Echo("쁘띠 096", "사용 시 15초간 시야 개선 및 스태미너 무제한, 재사용 대기시간 60초", EchoType.Chibi096, EchoCost.Cost3, EchoMainStatType.AttackPercent, "🐶")]
public class Chibi096 : Echo
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}