using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("구보", "5초간 아무런 아이템을 들 수 없는 대신, 이동 속도가 12% 증가합니다.", RankAbilityType.구보, RankCategory.시설_경비, "🎽")]
    public class 구보 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            void OnChangingItem(ChangingItemEventArgs ev)
            {
                if (ev.Player == Owner)
                    ev.IsAllowed = false;
            }

            Owner.CurrentItem = null;

            Exiled.Events.Handlers.Player.ChangingItem += OnChangingItem;

            Owner.AddEffect(EffectType.MovementBoost, 12, 5);

            Timing.CallDelayed(5, () =>
            {
                Exiled.Events.Handlers.Player.ChangingItem -= OnChangingItem;
            });
        }
    }
}
