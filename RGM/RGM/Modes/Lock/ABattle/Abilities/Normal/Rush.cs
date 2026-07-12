using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Normal;

[Ability("황소", "지급된 동전을 튕기면 1.5초 동안 속도가 압도적으로 빨라집니다.", AbilityCategory.Common, AbilityType.NORMAL_RUSH)]
public class Rush : Ability
{
    ushort Serial = 0;

    public override void OnEnabled()
    {
        Item coin = Owner.AddItem(ItemType.Coin);
        Serial = coin.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
    }

    public override void OnDisabled()
    {
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item != null)
        {
            if (Serial == ev.Item.Serial)
                ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["일반"]}>황소</color></b> 능력을 사용할 수 있습니다.");
        }
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (Serial == ev.Item.Serial)
        {
            ev.Item.Destroy();

            byte intensity = ev.Player.GetEffect(EffectType.MovementBoost).Intensity;
            float duration = ev.Player.GetEffect(EffectType.MovementBoost).Duration;

            ev.Player.EnableEffect(EffectType.MovementBoost, 255, 1.5f);

            Timing.CallDelayed(1.5f, () =>
            {
                ev.Player.EnableEffect(EffectType.MovementBoost, intensity, duration);
            });
        }
    }
}
