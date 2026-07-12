using Exiled.API.Enums;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("예능", "점프력이 20% 향상됩니다.", RankAbilityType.예능, RankCategory.튜토리얼, RankAbilityCategory.변칙성, "🎴")]
    public class 예능 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.AddEffect(EffectType.Lightweight, 20);
        }

        public override void OnDisabled()
        {
        }
    }
}
