namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost1;

[Echo("ClassD", "Cost 1 샘플 Echo", EchoType.ClassD, EchoCost.Cost1, EchoMainStatType.AttackPercent, "🟠")]
public class ClassD : Echo
{
    public override void OnEnabled()
    {
        // 패시브 스탯만 적용 (EchoStats에서 처리)
    }

    public override void OnDisabled()
    {
    }
}