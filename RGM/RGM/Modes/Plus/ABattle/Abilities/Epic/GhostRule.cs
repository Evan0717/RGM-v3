using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Epic;

[Ability("고스트룰", "유령이 되어 문을 통과할 수 있게 됩니다.", AbilityCategory.Epic, AbilityType.EPIC_GHOSTRULE)]
public class GhostRule : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.Ghostly, 1);
    }

    public override void OnDisabled()
    {
        Owner.RemoveEffect(EffectType.Ghostly, 1);
    }
}
