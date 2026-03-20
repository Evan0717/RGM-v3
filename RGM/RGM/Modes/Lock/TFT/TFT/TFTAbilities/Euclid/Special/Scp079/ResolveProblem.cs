using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

namespace DAONTFT.Core.TFT.Euclid.Scp079;

[TFTAbility("문제 해결사", "문들을 전부 고치고, 발전기들도 초기 상태로 되돌립니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp079, TFTAbilityPoint.Once, TFTAbilityType.ResolveProblem, "✅")]
public class ResolveProblem : TFTAbility
{
    public override void OnEnabled()
    {
        foreach (var door in Door.List)
        {
            if (door is BreakableDoor breakableDoor)
                breakableDoor.Repair();
        }

        foreach (var generator in Generator.List)
        {
            generator.State = GeneratorState.Unlocked;
        }
    }

    public override void OnDisabled()
    {
    }
}
