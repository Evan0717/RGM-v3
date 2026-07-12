using Exiled.API.Enums;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.기어
{
    [RankAbility("방어", "받는 데미지가 9% 줄어듭니다.", RankAbilityType.방어, RankCategory.공통, RankAbilityCategory.기어_메인, "📘")]
    public class 방어 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.AddEffect(EffectType.DamageReduction, 18);
        }

        public override void OnDisabled()
        {
            Owner.RemoveEffect(EffectType.DamageReduction, 18);
        }
    }
}
