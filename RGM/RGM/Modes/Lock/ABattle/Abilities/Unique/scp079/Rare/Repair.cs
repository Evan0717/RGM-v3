using Exiled.API.Features.Doors;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using Map = Exiled.API.Features.Map;

namespace RGM.Modes.Abilities.Unique.Scp079.Rare;

[Ability("수리수리 마수리", "1 ~ 2분마다 부서진 모든 문이 복구됩니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_REPAIR, RoleAbility.Scp079)]
public class Repair : Ability
{
    private CoroutineHandle _repairHandle;
    private static readonly System.Random Rand = new(Map.Seed);
    
    public override void OnEnabled()
    {
        if (!Timing.IsRunning(_repairHandle))
            _repairHandle = Timing.RunCoroutine(RepairCoroutine());
    }

    private static IEnumerator<float> RepairCoroutine()
    {
        while (true)
        {
            Log.Info("Running");
            foreach (var door in Door.List)
            {
                if (door is BreakableDoor breakableDoor) 
                    breakableDoor.Repair();
            }
            yield return Timing.WaitForSeconds(Rand.Next(1, 3) * 60);
        }
    }

    public override void OnDisabled() 
        => Timing.KillCoroutines(_repairHandle);
}
