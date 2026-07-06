using Exiled.Events.EventArgs.Player;
using RGM.Modes;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.기어
{
    [RankAbility("치유", "회복량이 50% 증가합니다.", RankAbilityType.치유, RankCategory.공통, RankAbilityCategory.기어_유틸, "♥")]
    public class 치유 : RankAbility
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Healing += OnHealing;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Healing -= OnHealing;
        }

        void OnHealing(HealingEventArgs ev)
        {
            if (ev.Player == Owner)
                ev.Amount = (int)(ev.Amount * 1.5f);
        }
    }
}
