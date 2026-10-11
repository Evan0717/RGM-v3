using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("토끼뜀", "점프력이 8%p 증가합니다.", AbilityCategory.Normal, AbilityType.NORMAL_RABBIT)]
public class Rabbit : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.Lightweight, 8);
    }
}