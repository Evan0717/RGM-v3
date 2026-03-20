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

namespace DAONTFT.Core.TFT.Keter.Scp079;

[TFTAbility("문제 해결사+", "발전기들을 초기 상태로 되돌립니다. 5분마다 문들이 원상복구됩니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Scp079, TFTAbilityPoint.Continuous, TFTAbilityType.ResolveProblem2, "✅")]
public class ResolveProblem2 : TFTAbility
{
    CoroutineHandle _fixDoors;

    public override void OnEnabled()
    {
        foreach (var generator in Generator.List)
        {
            generator.State = GeneratorState.Unlocked;
        }

        _fixDoors = Timing.RunCoroutine(fixDoors());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_fixDoors);
    }

    IEnumerator<float> fixDoors()
    {
        while (true)
        {
            foreach (var door in Door.List)
            {
                if (door is BreakableDoor breakableDoor)
                    breakableDoor.Repair();
            }

            yield return Timing.WaitForSeconds(300f);
        }
    }
}
