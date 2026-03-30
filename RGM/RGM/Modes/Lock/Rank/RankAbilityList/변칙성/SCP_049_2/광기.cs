using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.Patches.Events.Scp049;
using MEC;
using RGM.Modes;
using System.Collections.Generic;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("광기", "체력이 150 추가되고, SCP-207 효과를 얻습니다.", RankAbilityType.광기, RankCategory.SCP_049_2, RankAbilityCategory.변칙성, "😡")]
    public class 광기 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.MaxHealth += 150;
            Owner.Health = Owner.MaxHealth;

            Owner.AddEffect(EffectType.Scp207, 1);
        }

        public override void OnDisabled()
        {
        }
    }
}
