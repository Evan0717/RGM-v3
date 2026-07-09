using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost1;

[Echo("과학자", "Cost 1 샘플 Echo", EchoType.Scientist, EchoCost.Cost1, EchoMainStatType.HpPercent, "🟡")]
public class Scientist : Echo
{
    public override void OnEnabled()
    {
    }

    public override void OnDisabled()
    {
    }
}