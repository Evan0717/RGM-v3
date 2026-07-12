using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("공간이동", "지급된 동전을 튕기면 대상에게 이동합니다. (사거리 100)", AbilityCategory.Rare, AbilityType.RARE_SPACETRAVEL)]
public class SpaceTravel : Ability
{
    ushort serial = 0;

    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.Coin);
        serial = item.Serial;

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
            if (serial == ev.Item.Serial)
                ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["희귀"]}>공간이동</color></b> 능력을 사용할 수 있습니다.");
        }
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (serial == ev.Item.Serial)
        {
            if (Tools.TryGetLookPlayer(ev.Player, 100f, out Player player, out RaycastHit? hit))
            {
                ev.Player.Position = player.Position;

                ev.Item.Destroy();
                Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);
            }
            else
                ev.Player.AddHint("동전 사용 실패", "대상을 정확히 지정해 주세요.");
        }
    }
}
