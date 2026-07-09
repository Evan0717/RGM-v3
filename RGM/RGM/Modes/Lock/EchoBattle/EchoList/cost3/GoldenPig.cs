using System;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost3;

[Echo("황금 돼지", "사용 시 체력 1205 회복(최대치 초과 불가), 재사용 대기시간 60초", EchoType.GoldenPig, EchoCost.Cost3, EchoMainStatType.HpPercent, "🐷")]
public class GoldenPig : EchoActiveAbility
{
    public override float Duration => 0f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "체력 1205 회복";

    protected override void OnActiveUsed()
    {
        Owner.Health = Math.Min(Owner.Health + 1205f, Owner.MaxHealth);
    }
}