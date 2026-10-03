using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("바디백", "몸통 데미지 경감 효과가 15% 추가됩니다.",
    AbilityCategory.Normal, AbilityType.NORMAL_BODYBACK, isUnique:true)]
public class Bodyback : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.BodyshotReduction, 4);
    }
}
