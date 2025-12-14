using System;
using System.Collections.Generic;
using System.Linq;
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

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Rare;

[Ability("회중시계", "지급된 동전을 튕기면 3초간 움직일 수 없는 대신에 5초간 무적 상태가 됩니다.", AbilityCategory.Rare, AbilityType.RARE_STOPWATCH)]
public class StopWatch : Ability
{
    ushort ClockCoinSerial = 0;

    public override void OnEnabled()
    {
        Item cc = Owner.AddItem(ItemType.Coin);
        ClockCoinSerial = cc.Serial;

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
            if (ClockCoinSerial == ev.Item.Serial)
                ev.Player.AddHint("동전 사용 설명", $"이 동전을 튕기면 <b><color={ABattle.RatingColor["희귀"]}>회중시계</color></color></b> 능력을 사용할 수 있습니다.");
        } 
    }

    public void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        ushort Serial = ev.Item.Serial;

        if (ClockCoinSerial == Serial)
        {
            ev.Item.Destroy();

            ev.Player.EnableEffect(EffectType.Ensnared, 1, 3);

            GodModePlayers.Add(ev.Player);

            Timing.CallDelayed(5, () => 
            { 
                if (GodModePlayers.Contains(ev.Player)) 
                    GodModePlayers.Remove(ev.Player); 
            });
        }
    }
}
