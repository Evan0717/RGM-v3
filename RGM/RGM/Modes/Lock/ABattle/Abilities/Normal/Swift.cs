using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("경공", "이동 속도가 5% 증가합니다.", AbilityCategory.Common, AbilityType.NORMAL_SWIFT)]
public class Swift : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.MovementBoost, 5);
    }

    public override void OnDisabled()
    {
    }
}