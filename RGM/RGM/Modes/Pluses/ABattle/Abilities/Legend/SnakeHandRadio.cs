using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Legend;

[Ability("뱀의 손 무전기", "무전기를 든 상태로 우클릭하면 뱀의 손 지원을 부르며, 자신도 뱀의 손 소속이 됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_SNAKEHANDRADIO)]
public class SnakeHandRadio : Ability
{
    ushort CallSnakeHandsSerial = 0;

    public override void OnEnabled()
    {
        Item SH = Owner.AddItem(ItemType.Radio);
        CallSnakeHandsSerial = SH.Serial;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.TogglingRadio += OnTogglingRadio;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;
        Exiled.Events.Handlers.Player.TogglingRadio -= OnTogglingRadio;
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (ev.Item != null)
        {
            if (CallSnakeHandsSerial == ev.Item.Serial)
                ev.Player.ShowHint($"<b><color={ABattle.RatingColor["전설"]}>뱀의 손 무전기</color></b> 능력이 있는 <b>무전기</b>입니다!");
        }
    }

    public void OnTogglingRadio(TogglingRadioEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (CallSnakeHandsSerial == ev.Item.Serial)
        {
            ev.Item.Destroy();

            ev.Player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.None);

            Tools.CallSnakeHand(ev.Player, Player.List.Where(x => x.IsDead).ToList());
        }
    }
}
