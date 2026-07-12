using Exiled.API.Enums;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("스펀지", "15초간 발걸음 소리가 사라집니다.", RankAbilityType.스펀지, RankCategory.반란, "👢", 60)]
    public class 스펀지 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            Owner.AddEffect(EffectType.SilentWalk, 11, 15);
        }
    }
}
