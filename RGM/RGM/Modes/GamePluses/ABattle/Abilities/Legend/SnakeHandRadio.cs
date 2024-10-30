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


using static RGM.Modes.ABattleFunctions.SpecificAbilities;

namespace RGM.Modes.Abilities.Legend;

[Ability("뱀의 손 무전기", "무전기를 든 상태로 우클릭하면 뱀의 손 지원을 부르며, 자신도 뱀의 손 소속이 됩니다.", AbilityCategory.Legend, AbilityType.LEGEND_SNAKEHANDRADIO)]
public class SnakeHandRadio : Ability
{
    ushort CallSnakeHandsSerial = 0;

    public override void OnEnabled()
    {
        Item SH = Owner.AddItem(ItemType.Radio);
        CallSnakeHandsSerial = SH.Serial;

        if (Owner.IsScp)
            Owner.CurrentItem = SH;

        Exiled.Events.Handlers.Player.TogglingRadio += OnTogglingRadio;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.TogglingRadio -= OnTogglingRadio;
    }

    public void OnTogglingRadio(TogglingRadioEventArgs ev)
    {
        if (CallSnakeHandsSerial == ev.Item.Serial)
        {
            ev.Item.Destroy();

            ev.Player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.None);

            CallSnakeHand(ev.Player, Player.List.Where(x => x.IsDead).ToList());
        }
    }
}
