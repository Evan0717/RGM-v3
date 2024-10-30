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

[Ability("물의 정령, 운디네", "청량한 바다의 기운! 불, 흙, 바람의 정령을 모으면..?", AbilityCategory.Rare, AbilityType.RARE_UNDINE)]
public class Undine : Ability
{
    public override void OnEnabled()
    {
        Room Room = Owner.CurrentRoom;
        Color RoomColor = Owner.CurrentRoom.Color;

        Room.Color = new Color(0, 0, 1);

        Timing.CallDelayed(10f, () =>
        {
            Room.Color = RoomColor;
        });
    }

    public override void OnDisabled()
    {
    }
}
