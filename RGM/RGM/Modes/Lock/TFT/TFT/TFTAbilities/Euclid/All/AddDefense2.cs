using Exiled.API.Enums;
using RGM.API.Features;

namespace DAONTFT.Core.TFT.Euclid.All;

[TFTAbility("방어Ⅱ", "방어력을 얻습니다. (+20%)", TFTAbilityLevel.Euclid, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddDefense2, "⛔")]
public class AddDefense2 : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.DamageReduction, 40);
    }

    public override void OnDisabled()
    {
    }
}
