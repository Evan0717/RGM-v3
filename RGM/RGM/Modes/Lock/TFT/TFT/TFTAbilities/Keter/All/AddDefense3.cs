using Exiled.API.Enums;
using RGM.API.Features;

namespace DAONTFT.Core.TFT.Keter.All;

[TFTAbility("방어Ⅲ", "방어력을 30% 얻습니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddDefense3, "⛔")]
public class AddDefense3 : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.DamageReduction, 60);
    }

    public override void OnDisabled()
    {
    }
}
