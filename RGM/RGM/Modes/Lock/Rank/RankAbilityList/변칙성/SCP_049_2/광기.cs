using Exiled.API.Enums;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("광기", "체력이 100 추가되고, SCP-207 효과를 얻습니다. 단, 받는 피해가 25% 증가합니다.", RankAbilityType.광기, RankCategory.SCP_049_2, RankAbilityCategory.변칙성, "😡")]
    public class 광기 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.MaxHealth += 100;
            Owner.Health = Owner.MaxHealth;

            Owner.AddEffect(EffectType.Scp207, 1);
            Owner.AddEffect(EffectType.Burned, 1);
        }

        public override void OnDisabled()
        {
        }
    }
}
