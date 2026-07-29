using Exiled.API.Features.Doors;
using UnityEngine;
using System.Collections.Generic;
using MEC;
using Map = Exiled.API.Features.Map;

namespace RGM.Modes.Abilities.Unique.Scp079.Rare;

//[Ability("수리수리 마수리", "1 ~ 2분마다 부서진 모든 문이 각각 50% 확률로 복구됩니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_REPAIR, RoleAbility.Scp079)]
public class Repair : Ability
{
    private CoroutineHandle _repairHandle;
    private static readonly System.Random Rand = new(Map.Seed);
    
    public override void OnEnabled() 
        => _repairHandle = Timing.RunCoroutine(RepairCoroutine());

    private static IEnumerator<float> RepairCoroutine()
    {
        while (true)
        {
            foreach (var door in Door.List)
            {
                if (door is not BreakableDoor breakableDoor) continue;
                    
                if (Mathf.Clamp01((float)Rand.NextDouble()) <= .5f) { breakableDoor.Repair(); }
            }
            Timing.WaitForSeconds(Rand.Next(1, 3) * 60);
        }
    }

    public override void OnDisabled() 
        => Timing.KillCoroutines(_repairHandle);
}
