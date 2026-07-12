using Exiled.API.Enums;
using Exiled.Events.EventArgs.Scp3114;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("단백질", "변장 해제 후 이동 속도가 10초간 25% 증가합니다.", RankAbilityType.단백질, RankCategory.SCP_3114, RankAbilityCategory.변칙성, "🍢")]
    public class 단백질 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp3114.Revealed += OnRevealed;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp3114.Revealed -= OnRevealed;
        }

        void OnRevealed(RevealedEventArgs ev)
        {
            if (ev.Player == Owner)
                Owner.AddEffect(EffectType.MovementBoost, 25, 10);
        }
    }
}
