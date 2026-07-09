using Exiled.API.Enums;
using RGM.API.Features;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("목표를 포착했다", "15초 동안 시야가 개선되고 스테미나가 무제한이 됩니다.", RankAbilityType.목표를_포착했다, RankCategory.SCP_939, "🔭", 30)]
    public class 목표를_포착했다 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            Owner.AddEffect(EffectType.Invigorated, 1, 15);
        }
    }
}
