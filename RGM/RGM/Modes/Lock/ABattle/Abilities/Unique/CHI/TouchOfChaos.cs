using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.CHI;

[Ability("혼돈의 손길", "지급된 동전을 튕기면 보유한 능력을 전부 삭제합니다.", AbilityCategory.CHI, AbilityType.CHI_TOUCHOFCHAOS)]
public class TouchOfChaos : Ability
{
    ushort ChaosCoinSerial;

    public override void OnEnabled()
    {
        Item Ch = Owner.AddItem(ItemType.Coin);
        ChaosCoinSerial = Ch.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;
        Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item != null)
        {
            if (ChaosCoinSerial == ev.Item.Serial)
                ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["전용"]}>혼돈의 손길</color></color></b> 능력을 사용할 수 있습니다.");
        }
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (ev.Player.CurrentItem.Serial != ChaosCoinSerial)
            return;

        ev.Item.Destroy();

        ev.Player.DisableAllEffects();
        ev.Player.RemoveAllAbilities();

        ABattle.Instance.PlayerWorkstations[ev.Player].Clear();
        ABattle.Instance.PlayerAbilities[ev.Player].Clear();
    }
}
