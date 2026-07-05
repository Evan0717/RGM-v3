using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("잠행", "발걸음 소리가 5%만큼 감소합니다.", AbilityCategory.Common, AbilityType.NORMAL_SNEAK)]
public class Sneak : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.SilentWalk, 5);
    }

    public override void OnDisabled()
    {
        Owner.RemoveEffect(EffectType.SilentWalk, 5);
    }
}
