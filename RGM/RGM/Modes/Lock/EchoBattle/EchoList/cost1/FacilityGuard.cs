using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost1;

[Echo("시설 경비", "시설의 경비 인원입니다.", EchoType.FacilityGuard, EchoCost.Cost1, EchoMainStatType.Defense, "🟢")]
public class FacilityGuard : Echo
{
    public override void OnEnabled()
    {
    }

    public override void OnActiveEffect()
    {
    }
}