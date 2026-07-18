using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost1;

[Echo("과학자", "시설의 과학자입니다.", EchoType.Scientist, EchoCost.Cost1, EchoMainStatType.HpPercent, "🟡")]
public class Scientist : Echo
{
    public override void OnEnabled()
    {
    }

    public override void OnActiveEffect()
    {
    }
}