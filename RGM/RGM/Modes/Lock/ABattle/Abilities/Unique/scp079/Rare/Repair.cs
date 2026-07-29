using Exiled.API.Features.Doors;
using UnityEngine;
using System.Collections.Generic;
using MEC;

namespace RGM.Modes.Abilities.Unique.Scp079.Rare;

//[Ability("수리수리 마수리", "1 ~ 2분마다 부서진 모든 문이 각각 50% 확률로 복구됩니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_REPAIR, RoleAbility.Scp079)]
public class Repair : Ability
{
    CoroutineHandle RepairHandle;
    public override void OnEnabled()
    {
        IEnumerator<float> RepairCoroutine()
        {
            while (true)
            {
                foreach (var door in Door.List)
                {
                    if (door is BreakableDoor breakableDoor)
                    {
                        if (Random.Range(0, 2) == 1) { breakableDoor.Repair(); }
                    }
                }
                Timing.WaitForSeconds(Random.Range(1, 3) * 60);
            }
        }

        RepairHandle = Timing.RunCoroutine(RepairCoroutine());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(RepairHandle);   
    }
}
