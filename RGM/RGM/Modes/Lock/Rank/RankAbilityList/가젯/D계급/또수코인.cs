using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.Modes;
using RGM.API.Features;
using Exiled.API.Features.Items;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("또수코인", "코인을 획득합니다. 이 코인을 튕기면 이동 속도가 +3% 누적되지만, 10% 확률로 초기화됩니다.", RankAbilityType.또수코인, RankCategory.D계급, "🕜")]
    public class 또수_코인 : RankGadgetAbility
    {

        protected override void OnGadgetUsed()
        {
            Item item = Owner.AddItem(ItemType.Coin);

            void OnFlippingCoin(FlippingCoinEventArgs ev)
            {
                if (Owner == ev.Player && item == ev.Item)
                {
                    Owner.AddEffect(EffectType.MovementBoost, 3);
                    if (Random.Range(1, 101) <= 10)
                    {
                         Owner.DisableEffect(EffectType.MovementBoost);
                    }
                }

                Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;

                Timing.CallDelayed(2, item.Destroy);
            }

            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
        }
    }
}
