using System.Collections.Generic;
using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Legend;

[Ability("섬뜩한 힘", "SCP-330의 효과가 일괄적으로 적용됩니다. 체력을 500 얻습니다.", 
    AbilityCategory.Legend, AbilityType.LEGEND_CANDYPOWER, RoleAbility.None, false, AbilityHolidayType.Halloween)]
public class CandyPower : Ability
{
    private readonly List<EffectType> _effects =
    [
        EffectType.SugarHigh,
        EffectType.Prismatic,
        EffectType.OrangeCandy,
        EffectType.WhiteCandy,
        EffectType.Metal,
        EffectType.MovementBoost,
        EffectType.Lightweight,
        EffectType.Spicy
    ];
    
    public override void OnEnabled()
    {
        _effects.ForEach(effect => Owner.AddEffect(effect, 90));

        Owner.MaxHealth += 500;
        Owner.Health += 500;
    }
}
