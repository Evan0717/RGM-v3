using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.Modes;
using RGM.API.Features;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("구보", "8초간 아무런 아이템을 들 수 없는 대신, 이동 속도가 40% 증가합니다. 이후, 1.5초간 움직일 수 없습니다.", RankAbilityType.구보, RankCategory.시설_경비, "🎽", 60)]
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

            Owner.AddEffect(EffectType.MovementBoost, 40, 8);

            Timing.CallDelayed(8, () =>
            {
                Exiled.Events.Handlers.Player.ChangingItem -= OnChangingItem;
                Owner.AddEffect(EffectType.Ensnared, 1, 1.5f);
            });
        }
    }
}
