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

namespace RGM.Modes.Abilities.Normal;

[Ability("트릭 오어 트릿", "랜덤한 SCP-330을 받습니다. 운이 좋다면 더 받을수도 있겠죠..", AbilityCategory.Common, AbilityType.NORMAL_RANDOMCANDY)]
public class RandomCandy : Ability
{
    public override void OnEnabled()
    {
        Owner.AddRandomCandy();

        if (Random.Range(1, 8) == 1)
            Owner.AddRandomCandy();
    }

    public override void OnDisabled()
    {
    }
}
