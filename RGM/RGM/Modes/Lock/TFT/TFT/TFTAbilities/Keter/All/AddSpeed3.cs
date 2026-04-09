using Exiled.API.Enums;
using RGM.API.Features;

namespace DAONTFT.Core.TFT.Keter.All;

[TFTAbility("경공Ⅲ", "더 빠르게 이동합니다. (+30%)", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddSpeed3, "🏃")]
public class AddSpeed3 : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.MovementBoost, 30);
    }

    public override void OnDisabled()
    {
    }
}
