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

namespace RGM.Modes.Abilities.Epic;

[Ability("럭키비키", "이전에 방문했던 워크스테이션에서 다시 한번 더 능력을 획득할 수 있습니다.", AbilityCategory.Epic, AbilityType.EPIC_LUCKYVIKEY)]
public class LuckyVicky : Ability
{
    public override void OnEnabled()
    {
        ABattle.Instance.PlayerWorkstations[Owner].Clear();
    }

    public override void OnDisabled()
    {
    }
}
