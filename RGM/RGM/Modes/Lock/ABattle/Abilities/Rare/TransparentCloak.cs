using Exiled.API.Enums;

namespace RGM.Modes.Abilities.Rare;

[Ability("투명 망토", "25초 간 투명 효과를 받습니다.", AbilityCategory.Rare, AbilityType.RARE_TRANSPARENTCLOAK)]
public class TransparentCloak : Ability
{
    public override void OnEnabled()
    {
        Owner.EnableEffect(EffectType.Invisible, 1, 25);
    }

    public override void OnDisabled()
    {
        Owner.DisableEffect(EffectType.Invisible);
    }
}
