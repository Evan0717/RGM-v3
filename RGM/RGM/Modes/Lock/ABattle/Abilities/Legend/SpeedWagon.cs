using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Legend;

[Ability("스피드왜건", "이동 속도가 105% 증가합니다.", AbilityCategory.Legend, AbilityType.LEGEND_SPEEDWAGON)]
public class SpeedWagon : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.MovementBoost, 105);
    }

    public override void OnDisabled()
    {
    }
}
