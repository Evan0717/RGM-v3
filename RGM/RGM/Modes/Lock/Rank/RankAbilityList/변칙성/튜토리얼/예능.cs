using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("예능", "점프력이 30% 향상됩니다.", RankAbilityType.예능, RankCategory.튜토리얼, RankAbilityCategory.변칙성, "🎴")]
    public class 예능 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.AddEffect(EffectType.Lightweight, 30);
        }

        public override void OnDisabled()
        {
        }
    }
}
