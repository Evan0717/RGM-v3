using Exiled.API.Enums;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("암살 조장", "발걸음 소리가 줄어들고 흐리게 보입니다.", RankAbilityType.암살_조장, RankCategory.SCP_939, RankAbilityCategory.변칙성, "📛")]
    public class 암살_조장 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.AddEffect(EffectType.SilentWalk, 9);
            Owner.AddEffect(EffectType.Fade, 125);
        }

        public override void OnDisabled()
        {
        }
    }
}
