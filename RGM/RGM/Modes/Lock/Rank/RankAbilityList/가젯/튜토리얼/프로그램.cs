using Exiled.API.Enums;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("프로그램", "10초 동안 점프력이 50% 추가로 향상됩니다.", RankAbilityType.프로그램, RankCategory.튜토리얼, "💽", 60)]
    public class 프로그램 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            Owner.AddEffect(EffectType.Lightweight, 50, 10);
        }
    }
}
