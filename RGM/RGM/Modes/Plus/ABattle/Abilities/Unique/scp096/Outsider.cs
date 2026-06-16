using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp096;

[Ability("아웃사이더", "몸이 25% 투명해집니다.", AbilityCategory.Scp096, AbilityType.SCP096_OUTSIDER)]
public class Outsider : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.Fade, 64);
    }

    public override void OnDisabled()
    {
    }
}
