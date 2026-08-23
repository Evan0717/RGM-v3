using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.ClassD.Rare;

[Ability("스피드런", "즉시 지상으로 순간이동합니다.",
    AbilityCategory.Rare, AbilityType.RARE_CLASSD_CLASSDSPEEDRUN, RoleAbility.ClassD)]

public class ClassDSpeedRun : Ability
{
    public override void OnEnabled()
    {
        Owner.Position = Door.Get(DoorType.SurfaceGate).Position + Vector3.up * 1.5f;
    }
}