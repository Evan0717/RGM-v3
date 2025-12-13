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

[Ability("뱀의 손 무전기", "무전기를 든 상태로 우클릭하면 뱀의 손 지원을 부르며, 자신도 뱀의 손 소속이 됩니다. 이후 럭키비키를 획득합니다.", AbilityCategory.Legend, AbilityType.LEGEND_SNAKEHANDRADIO)]
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
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item != null)
        {
            if (CallSnakeHandsSerial == ev.Item.Serial)
                ev.Player.AddHint("뱀의 손 무전기", $"<b><color={ABattle.RatingColor["전설"]}>뱀의 손 무전기</color></b> 능력이 있는 <b>무전기</b>입니다!");
        }
    }

    public void OnTogglingRadio(TogglingRadioEventArgs ev)
    {
        if (CallSnakeHandsSerial == ev.Item.Serial)
        {
            ev.Item.Destroy();

            ev.Player.RemoveAbility(this);
            ev.Player.AddAbility(AbilityType.DUMMY_USEDSNAKEHANDRADIO);

            if (ev.Player.Role.Type != RoleTypeId.Tutorial)
                ev.Player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.None);

            List<Player> deadPlayers = PlayerManager.List.Where(x => x.IsDead).ToList();
            Tools.CallSnakeHand(ev.Player, deadPlayers);

            Timing.CallDelayed(1, () =>
            {
                ev.Player.Position = deadPlayers.FirstOrDefault().Position;
            });

            ev.Player.AddAbility(AbilityType.EPIC_LUCKYVIKEY);
        }
    }
}
