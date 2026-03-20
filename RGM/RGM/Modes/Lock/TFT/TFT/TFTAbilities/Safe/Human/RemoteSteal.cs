using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

namespace DAONTFT.Core.TFT.Safe.Human;

[TFTAbility("슬쩍하기", "무작위 유저가 가진 아이템을 하나 복사하여 획득합니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.RemoteSteal)]
public class RemoteSteal : TFTAbility
{
    public override void OnEnabled()
    {
        List<ItemType> items = new();

        foreach (var player in Player.List.Where(x => x != Owner && x.IsAlive && !x.IsScp))
        {
            foreach (var item in player.Items.Where(x => !x.IsAmmo))
            {
                items.Add(item.Type);
            }
        }

        if (items.Count == 0)
            items.Add(ItemType.DebugRagdollMover);

        Owner.AddItem(items.GetRandomValue());
    }

    public override void OnDisabled()
    {
    }
}
