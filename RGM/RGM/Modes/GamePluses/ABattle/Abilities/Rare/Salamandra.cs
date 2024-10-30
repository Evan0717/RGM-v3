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

[Ability("살라만드라", "뜨거운 열정의 기운! 물, 흙, 바람의 정령을 모으면..?", AbilityCategory.Rare, AbilityType.RARE_SALAMANDRA)]
public class Salamandra : Ability
{
    public override void OnEnabled()
    {
        Room Room = Owner.CurrentRoom;
        Color RoomColor = Owner.CurrentRoom.Color;

        Room.Color = new Color(1, 0, 0);

        Timing.CallDelayed(10f, () =>
        {
            Room.Color = RoomColor;
        });
    }

    public override void OnDisabled()
    {
    }
}
