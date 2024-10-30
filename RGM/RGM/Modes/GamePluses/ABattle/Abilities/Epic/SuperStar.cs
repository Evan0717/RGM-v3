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

[Ability("슈퍼 스타", "자신의 마이크가 모두에게 공유되고, 사망 시 사망한 사실이 모두에게 공개됩니다.", AbilityCategory.Epic, AbilityType.EPIC_SUPERSTAR)]
public class SuperStar : Ability
{
    public override void OnEnabled()
    {
        Server.ExecuteCommand($"/speak {Owner.Id} 1");
    }

    public override void OnDisabled()
    {
        Server.ExecuteCommand($"/speak {Owner.Id} 0");
    }
}
