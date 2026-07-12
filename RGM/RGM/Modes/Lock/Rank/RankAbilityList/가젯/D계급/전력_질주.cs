using Exiled.API.Enums;
using MEC;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("전력 질주", "5초간 빠르게 이동합니다. 이후 2초간 이동 속도가 감소합니다.", RankAbilityType.전력_질주, RankCategory.D계급, "💨", 60)]
    public class 전력_질주 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            Owner.AddEffect(EffectType.MovementBoost, 60, 5);
            Timing.CallDelayed(5f, () =>
                {
                    Owner.AddEffect(EffectType.SinkHole, 2, 2);
                }
            );
        }
    }
}
