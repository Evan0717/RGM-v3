using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost3;

[Echo("쁘띠 댕댕이", "사용 시 15초간 시야 개선 및 스태미너 무제한, 재사용 대기시간 60초", EchoType.PetitDangdang, EchoCost.Cost3, EchoMainStatType.StaminaDrainReduction, "🐶")]
public class PetitDangdang : EchoActiveAbility
{
    public override float Duration => 15f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "15초간 시야 개선 + 스태미나 무제한";

    protected override void OnActiveUsed()
    {
        // Rank SCP-939 가젯 '목표를 포착했다' 참고
        Owner.AddEffect(EffectType.Invigorated, 1, Duration);
        Owner.EnableEffect(EffectType.FogControl, 1, Duration);
    }
}