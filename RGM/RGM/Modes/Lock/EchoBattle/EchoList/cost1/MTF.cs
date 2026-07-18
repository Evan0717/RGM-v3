using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost1;

[Echo("MTF", "시설의 기동특무부대입니다.", EchoType.Mtf, EchoCost.Cost1, EchoMainStatType.AttackPercent, "🟠")]

public class MTF : Echo
{
    public override void OnEnabled()
    {
    }

    public override void OnActiveEffect()
    {
    }
}