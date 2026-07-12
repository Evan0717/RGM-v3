using Exiled.API.Enums;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.기어
{
    [RankAbility("속도", "이동 속도가 6% 증가합니다.", RankAbilityType.속도, RankCategory.공통, RankAbilityCategory.기어_유틸, "🏃")]
    public class 속도 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.AddEffect(EffectType.MovementBoost, 6);
        }

        public override void OnDisabled()
        {
            Owner.RemoveEffect(EffectType.MovementBoost, 6);
        }
    }
}
