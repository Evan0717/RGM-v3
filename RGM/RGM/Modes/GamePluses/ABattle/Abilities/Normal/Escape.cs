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

[Ability("위기 탈출", "넘버원! 지급된 동전을 튕기면 대상을 잠시 동안 멈추게 만듭니다.", AbilityCategory.Common, AbilityType.NORMAL_ESCAPE)]
public class Escape : Ability
{
    ushort EscapeCoinSerial = 0;

    public override void OnEnabled()
    {
        Item ec = Owner.AddItem(ItemType.Coin);
        EscapeCoinSerial = ec.Serial;

        if (Owner.IsScp)
            Owner.CurrentItem = ec;

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
        if (ev.Player != Owner)
            return;

        if (ev.Item != null)
        {
            if (EscapeCoinSerial == ev.Item.Serial)
                ev.Player.ShowHint($"이 동전을 튕기면 <b><color={ABattle.RatingColor["일반"]}>위기 탈출</color></b> 능력을 사용할 수 있습니다.");
        }
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        ushort Serial = ev.Item.Serial;

        if (EscapeCoinSerial == Serial)
        {
            if (Tools.TryGetLookPlayer(ev.Player, 10f, out Player player))
            {
                if (player.LeadingTeam != ev.Player.LeadingTeam)
                {
                    ev.Item.Destroy();

                    player.EnableEffect(EffectType.Ensnared, 3f);

                    Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);
                }
                else
                    ev.Player.ShowHint("잘못된 대상입니다.");
            }
            else
                ev.Player.ShowHint("대상을 정확히 지정해 주세요.");
        }
    }
}
