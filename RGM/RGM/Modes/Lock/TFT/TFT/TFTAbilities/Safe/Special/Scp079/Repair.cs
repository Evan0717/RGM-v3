using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

namespace DAONTFT.Core.TFT.Safe.Scp079;

[TFTAbility("수리 업체", "부서진 모든 문이 복구됩니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.Scp079, TFTAbilityPoint.Once, TFTAbilityType.Repair, "🔧")]
public class Repair : TFTAbility
{
    public override void OnEnabled()
    {
        foreach (var door in Door.List)
        {
            if (door is BreakableDoor breakableDoor)
                breakableDoor.Repair();
        }
    }

    public override void OnDisabled()
    {
    }
}
