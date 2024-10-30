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

namespace RGM.Modes.Abilities.Rare;

[Ability("갈고리", "지급된 동전을 튕기면 랜덤한 1인을 끌어옵니다.", AbilityCategory.Rare, AbilityType.RARE_GRAPPLINGHOOK)]
public class GrapplingHook : Ability
{
    public override void OnEnabled()
    {
        Player target1 = Tools.GetRandomValue(Player.List.Where(x => x.IsAlive && x != Owner && x.Role.Type != RoleTypeId.Scp079).ToList());
        target1.Position = Owner.Position;
    }

    public override void OnDisabled()
    {
    }
}
