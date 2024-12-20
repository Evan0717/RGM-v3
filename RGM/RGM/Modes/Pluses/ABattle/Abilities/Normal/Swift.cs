using System.Collections.Generic;
using Exiled.API.Enums;

namespace RGM.Modes.Abilities.Normal;

[Ability("경공", "이동 속도가 6% 증가합니다.", AbilityCategory.Common, AbilityType.NORMAL_SWIFT)]
public class Swift : Ability
{
    public override void OnEnabled()
    {
        Owner.GetEffect(EffectType.MovementBoost).Intensity += 6;
    }

    public override void OnDisabled()
    {
        Owner.GetEffect(EffectType.MovementBoost).Intensity -= 6;
    }
}