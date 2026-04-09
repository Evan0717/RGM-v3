using Exiled.API.Enums;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("전력 질주", "0.5초간 빠르게 이동합니다.", RankAbilityType.전력_질주, RankCategory.D계급, "💨")]
    public class 전력_질주 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            Owner.AddEffect(EffectType.MovementBoost, 70, 0.5f);
        }
    }
}
