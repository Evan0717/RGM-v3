using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("흙의 정령, 노움", "웅장한 대지의 기운! 불, 물, 바람의 정령을 모으면..?", AbilityCategory.Rare, AbilityType.RARE_GNOME)]
public class Gnome : Ability
{
    public override void OnEnabled()
    {
        Room Room = Owner.CurrentRoom;
        Color RoomColor = Owner.CurrentRoom.Color;

        Room.Color = new Color(0.5f, 0.25f, 0);

        Timing.CallDelayed(10f, () =>
        {
            Room.Color = RoomColor;
        });
    }

    public override void OnDisabled()
    {
    }
}
