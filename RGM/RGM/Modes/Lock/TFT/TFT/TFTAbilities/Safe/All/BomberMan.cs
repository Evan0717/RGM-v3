using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Safe.All;

[TFTAbility("봄버맨", "랜덤한 유저의 위치에 고폭 수류탄을 투척합니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.BomberMan, "💣")]
public class BomberMan : TFTAbility
{
    public override void OnEnabled()
    {
        var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Owner);
        g.FuseTime = 3f;
        g.SpawnActive(Player.List.Where(x => x.IsAlive && x.Role.Team != Owner.Role.Team && Owner != x).GetRandomValue().Position, Owner);
    }

    public override void OnDisabled()
    {
    }
}
