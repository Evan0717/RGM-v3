using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Rare;

[Ability("사탕 봉지", "랜덤한 사탕을 1~3개 받습니다.", AbilityCategory.Rare, AbilityType.RARE_CANDYBAG, AbilityHolidayType.Halloween)]
public class CandyBag : Ability
{
    public override void OnEnabled()
    {
        for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++)
        {
            Owner.AddCandy(Tools.EnumToList<CandyKindID>().GetRandomValue());
        }
    }

    public override void OnDisabled()
    {
    }
}
