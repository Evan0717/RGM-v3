using Exiled.API.Features.Doors;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("수리수리 마수리", "부서진 모든 문이 복구됩니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_REPAIR, RoleAbility.Scp079)]
public class Repair : Ability
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
