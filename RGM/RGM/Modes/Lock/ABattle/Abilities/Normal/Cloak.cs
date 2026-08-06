using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("망토", "투명도가 10%p 증가합니다.", AbilityCategory.Common, AbilityType.NORMAL_CLOAK)]
public class Cloak : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.Fade, 25);
    }

    public override void OnDisabled()
    {
        Owner.RemoveEffect(EffectType.Fade, 25);
    }
}