using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("우유", "지급된 동전을 튕기면 현재 자신에게 적용된 모든 효과를 제거합니다.", AbilityCategory.Common, AbilityType.NORMAL_MILK)]
public class Milk : Ability
{
    ushort CoinSerial = 0;

    public override void OnEnabled()
    {
        Item item = Owner.AddItem(ItemType.Coin);
        CoinSerial = item.Serial;

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
            if (CoinSerial == ev.Item.Serial)
                ev.Player.ShowHint($"이 동전을 튕기면 <b><color={ABattle.RatingColor["일반"]}>우유</color></b> 능력을 사용할 수 있습니다.");
        }
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (CoinSerial == ev.Item.Serial)
        {
            ev.Player.DisableAllEffects();
        }
    }
}
