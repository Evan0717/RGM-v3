using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.Modes;
using RGM.API.Features;
using Exiled.API.Features.Items;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("또수코인", "코인을 획득합니다. 이 코인을 튕기면 1% 빨라지거나, 1% 느려집니다.", RankAbilityType.또수코인, RankCategory.D계급, "🕜")]
    public class 또수_코인 : RankGadgetAbility
    {

        protected override void OnGadgetUsed()
        {
            Item item = Owner.AddItem(ItemType.Coin);

            void OnFlippingCoin(FlippingCoinEventArgs ev)
            {
                if (Owner == ev.Player && item == ev.Item)
                {
                    if (ev.IsTails)
                        Owner.AddEffect(EffectType.MovementBoost, 1);

                    else
                        Owner.RemoveEffect(EffectType.MovementBoost, 1);
                }

                Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;

                Timing.CallDelayed(2, item.Destroy);
            }

            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
        }
    }
}
