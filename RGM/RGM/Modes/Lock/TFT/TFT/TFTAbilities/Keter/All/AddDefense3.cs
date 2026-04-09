using Exiled.API.Enums;
using RGM.API.Features;

namespace DAONTFT.Core.TFT.Keter.All;

[TFTAbility("방어Ⅲ", "방어력을 얻습니다. (+40%)", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddDefense3, "⛔")]
public class AddDefense3 : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.DamageReduction, 80);
    }

    public override void OnDisabled()
    {
    }
}
