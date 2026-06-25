using Exiled.API.Enums;
using RGM.API.Features;

namespace DAONTFT.Core.TFT.Euclid.All;

[TFTAbility("경공Ⅱ", "더 빠르게 이동합니다. (+23%)", TFTAbilityLevel.Euclid, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddSpeed2, "🏃")]
public class AddSpeed2 : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.MovementBoost, 23);
    }

    public override void OnDisabled()
    {
    }
}
