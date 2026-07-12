using Exiled.API.Enums;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost4;

[Echo("노움", "사용 시 5초간 데미지 감소 70%, 재사용 대기시간 60초", EchoType.Gnome, EchoCost.Cost4, EchoMainStatType.Defense, "🪨")]
public class Gnome : EchoActiveAbility
{
    public override float Duration => 5f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "10초간 데미지 감소 70%";

    protected override void OnActiveUsed()
    {
        // DamageReduction intensity ≈ percent * 2 (Rank 방어 참고)
        Owner.AddEffect(EffectType.DamageReduction, 140, Duration);
    }
}