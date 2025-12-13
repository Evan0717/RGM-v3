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

[Ability("봄버맨", "랜덤한 유저의 위치에 고폭 수류탄을 투척합니다.", AbilityCategory.Rare, AbilityType.RARE_BOMBERMAN)]
public class BomberMan : Ability
{
    public override void OnEnabled()
    {
        var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Owner);
        g.FuseTime = 3f;
        g.SpawnActive(Tools.GetRandomValue(PlayerManager.List.ToList().Where(x => x.IsAlive && x.Role.Team != Owner.Role.Team && Owner != x).ToList()).Position, Owner);
    }

    public override void OnDisabled()
    {
    }
}
