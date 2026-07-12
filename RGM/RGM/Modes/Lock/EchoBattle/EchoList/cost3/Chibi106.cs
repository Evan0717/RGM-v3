using Exiled.API.Features.Roles;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.EchoBattle.EchoList.cost3;

[Echo("쁘띠 106", "SCP-106이 사용 시 기력 70% 회복, 재사용 대기시간 60초", EchoType.Chibi106, EchoCost.Cost3, EchoMainStatType.AhpRegenAndMax, "🐶")]
public class Chibi106 : EchoActiveAbility
{
    public override float Duration => 0f;
    public override float Cooldown => 60f;
    public override string ActiveDescription => "기력 70% 회복";

    protected override bool CanUseActive()
    {
        return base.CanUseActive()
               && Owner.Role is Scp106Role scp106
               && scp106.Vigor < scp106.VigorAbility.Vigor.MaxValue;
    }

    protected override void OnActiveUsed()
    {
        if (Owner.Role is Scp106Role scp106)
            scp106.Vigor = System.Math.Min(
                scp106.Vigor + scp106.VigorAbility.Vigor.MaxValue * 0.7f,
                scp106.VigorAbility.Vigor.MaxValue);
    }
}