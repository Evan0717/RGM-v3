using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("바디백", "몸통 데미지 경감 효과가 15% 추가됩니다(중복 불가능).", AbilityCategory.Common, AbilityType.NORMAL_BODYBACK)]
public class Bodyback : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.BodyshotReduction, 4);
    }

    public override void OnDisabled()
    {
        Owner.RemoveEffect(EffectType.BodyshotReduction, 4);
    }
}
