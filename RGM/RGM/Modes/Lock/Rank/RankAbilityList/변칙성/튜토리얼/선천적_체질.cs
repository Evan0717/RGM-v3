using Exiled.API.Enums;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("선천적 체질", "바디샷 감소 및 데미지 감소 효과를 15% 얻습니다.", RankAbilityType.선천적_체질, RankCategory.튜토리얼, RankAbilityCategory.변칙성, "💪")]
    public class 선천적_체질 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.AddEffect(EffectType.BodyshotReduction, 4);
            Owner.AddEffect(EffectType.DamageReduction, 30);
        }

        public override void OnDisabled()
        {
        }
    }
}
