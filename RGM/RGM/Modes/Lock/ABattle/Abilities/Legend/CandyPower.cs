using System.Collections.Generic;
using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Legend;

[Ability("섬뜩한 힘", "모든 SCP-330의 효과가 일괄적으로 적용됩니다. 체력을 200 얻습니다.", AbilityCategory.Legend, AbilityType.LEGEND_CANDYPOWER, AbilityHolidayType.Halloween)]
public class CandyPower : Ability
{
    public override void OnEnabled()
    {
        var effects = new List<EffectType>
        {
            EffectType.SugarRush,
            EffectType.Prismatic,
            EffectType.OrangeCandy,
            EffectType.WhiteCandy,
            EffectType.Metal,
            EffectType.MovementBoost,
            EffectType.Spicy,
        };

        foreach (var effect in effects)
        {
            Owner.AddEffect(effect, 50);
        }

        Owner.MaxHealth += 200;
        Owner.Health += 200;
    }

    public override void OnDisabled() 
    {
    }
}
