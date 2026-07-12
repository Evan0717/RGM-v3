using Exiled.API.Enums;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("밤눈", "야간 투시 효과가 적용됩니다.", RankAbilityType.밤눈, RankCategory.D계급, RankAbilityCategory.변칙성, "👀")]
    public class 밤눈 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.AddEffect(EffectType.NightVision, 20);
        }

        public override void OnDisabled()
        {
            Owner.RemoveEffect(EffectType.NightVision, 20);
        }
    }
}
