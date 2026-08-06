using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("밤눈", "NightVision 효과를 10p 획득합니다.", AbilityCategory.Common, AbilityType.NORMAL_NIGHTOWL)]
public class NightOwl : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.NightVision, 10);
    }

    public override void OnDisabled()
    {
        Owner.RemoveEffect(EffectType.NightVision, 10);
    }
}