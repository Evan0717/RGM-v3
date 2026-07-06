using Exiled.API.Enums;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("용도 외 사용금지", "8초간 투시 효과를 얻습니다.", RankAbilityType.용도_외_사용금지, RankCategory.구미호, "👓", 120)]
    public class 용도_외_사용금지 : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Owner.IsEffectActive<CustomPlayerEffects.Scp1344>())
                return false;

            else
                return true;
        }
        protected override void OnGadgetUsed()
        {
            Owner.AddEffect(EffectType.Scp1344, 1, 8);
        }
    }
}
