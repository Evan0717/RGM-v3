using Exiled.API.Enums;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.기어
{
    [RankAbility("속도", "이동 속도가 5% 증가합니다.", RankAbilityType.속도, RankCategory.공통, RankAbilityCategory.기어, "🏃")]
    public class 속도 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.AddEffect(EffectType.MovementBoost, 5);
        }

        public override void OnDisabled()
        {
            Owner.RemoveEffect(EffectType.MovementBoost, 5);
        }
    }
}
