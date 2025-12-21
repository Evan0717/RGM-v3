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

[Ability("실프", "싱그러운 바람의 기운! 물, 불, 흙의 정령을 모으면..?", AbilityCategory.Rare, AbilityType.RARE_SYLPH)]
public class Sylph : Ability
{
    public override void OnEnabled()
    {

        RoomType roomType = Owner.CurrentRoom.Type;
        Color roomColor = Owner.CurrentRoom.Color;

        Room room = Room.Get(roomType);

        room.Color = new Color(0, 1, 0);

        Timing.CallDelayed(10f, () =>
        {
            room.Color = roomColor;
        });
    }

    public override void OnDisabled()
    {
    }
}
