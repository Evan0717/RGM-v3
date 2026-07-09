using Exiled.API.Enums;
using Exiled.Events.EventArgs.Scp173;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("좋아, 자연스러웠어!", "순간이동 시, 0.8초 동안 은신 상태가 됩니다.", RankAbilityType.좋아_자연스러웠어, RankCategory.SCP_173, RankAbilityCategory.변칙성, "🎬")]
    public class 좋아_자연스러웠어 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;
        }

        void OnBlinking(BlinkingEventArgs ev)
        {
            if (ev.Player == Owner)
                ev.Player.AddEffect(EffectType.Invisible, 1, 0.8f);
        }
    }
}
