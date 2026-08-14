using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Rare;

[Ability("강철 껍질", "데미지 경감 효과가 5%p 추가됩니다.", AbilityCategory.Rare, AbilityType.RARE_STEELSHELL)]
public class SteelShell : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.DamageReduction, 10);
    }
}
