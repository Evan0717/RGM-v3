using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("이차원 도약", "지급된 동전을 튕기면 대상과 본인의 위치를 서로 뒤바꿉니다. (사거리 10)", AbilityCategory.Rare, AbilityType.RARE_SPACETRAVEL)]
public class SpaceTravel : Ability
{
    private ushort _serial;

    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.Coin);
        _serial = item.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item?.Serial != _serial)
            return;
        
        ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["희귀"]}>이차원 도약</color></b> 능력을 사용할 수 있습니다.");
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (_serial == ev.Item.Serial)
        {
            if (ev.Player.TryGetLookPlayer(10f, out Player player, out RaycastHit? hit))
            {
                Vector3 ownerPos = ev.Player.Position;
                Vector3 targetPos = player.Position;

                ev.Player.Position = targetPos;
                player.Position = ownerPos;

                ev.Item.Destroy();
                Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);
            }
            else
                ev.Player.AddHint("동전 사용 실패", "대상을 정확히 지정해 주세요.");
        }
    }
}
