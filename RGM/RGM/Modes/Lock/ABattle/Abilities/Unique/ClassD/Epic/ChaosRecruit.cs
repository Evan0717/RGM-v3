using MEC;
using PlayerRoles;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.ClassD.Epic;

[Ability("징집", 
    "즉시 CHAOS 징집병으로 변경하며, 워크스테이션 이용 기록을 초기화합니다.",
    AbilityCategory.Epic, AbilityType.EPIC_CLASSD_CHAOSRECRUIT, RoleAbility.ClassD)]

public class ChaosRecruit : Ability
{
    public override void OnEnabled()
    {
        Vector3 pos = Owner.Position;

        Timing.CallDelayed(0.1f, () =>
        {
            if (!Owner.IsDead) return;
            Owner.Role.Set(RoleTypeId.ChaosConscript, RoleSpawnFlags.AssignInventory);
            Owner.Position = pos;
            Owner.AddAbility(AbilityType.EPIC_LUCKYVIKEY);
        });
    }
}