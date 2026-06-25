using Exiled.API.Enums;
using RGM.API.Features;

namespace DAONTFT.Core.TFT.Safe.All;

[TFTAbility("종아리Ⅰ", "12%만큼 더 높이 점프할 수 있습니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddJump1, "🐎")]
public class AddJump1 : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.Lightweight, 12);
    }

    public override void OnDisabled()
    {
    }
}
