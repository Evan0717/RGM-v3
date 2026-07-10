using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("위기 탈출", "넘버원! 지급된 동전을 튕기면 대상을 잠시 동안 멈추게 만듭니다. (사거리 25)", AbilityCategory.Common, AbilityType.NORMAL_ESCAPE)]
public class Escape : Ability
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
                ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["일반"]}>위기 탈출</color></b> 능력을 사용할 수 있습니다.");
        }
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (serial == ev.Item.Serial)
        {
            if (Tools.TryGetLookPlayers(ev.Player, 25f, out List<Player> players, out RaycastHit? hit))
            {
                bool enemy = false;

                foreach (var player in players)
                {
                    if (HitboxIdentity.IsEnemy(ev.Player.ReferenceHub, player.ReferenceHub))
                    {
                        player.EnableEffect(EffectType.Ensnared, 3f);

                        enemy = true;
                    }
                }

                if (!enemy)
                {
                    ev.Player.AddHint("동전 사용 실패", "잘못된 대상입니다.");
                }
                else
                {
                    ev.Item.Destroy();
                    Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);
                }
            }
            else
                ev.Player.AddHint("동전 사용 실패", "대상을 정확히 지정해 주세요.");
        }
    }
}
