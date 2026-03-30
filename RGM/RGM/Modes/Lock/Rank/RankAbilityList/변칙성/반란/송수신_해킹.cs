using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("송수신 해킹", "무전기가 지급됩니다.", RankAbilityType.송수신_해킹, RankCategory.반란, RankAbilityCategory.변칙성, "📻")]
    public class 송수신_해킹 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.AddItem(ItemType.Radio);
        }

        public override void OnDisabled()
        {
        }
    }
}
