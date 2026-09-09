using Exiled.API.Enums;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Mythic;

[Ability("철퇴 자크", """
                  점프하는 순간 일대가 초토화됩니다.
                  착지 지점 반경 16m 이내의 모든 적은 『죽음에 이르는 공격』을 받습니다.
                  """, 
    AbilityCategory.Mythic, AbilityType.MYTHIC_HAMMER, RoleAbility.None, false, AbilityHolidayType.Halloween)]
public class Hammer : Ability
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.Metal, 255);
        Owner.AddEffect(EffectType.Lightweight, 255);
        Owner.AddEffect(EffectType.DamageReduction, 80);
    }
}