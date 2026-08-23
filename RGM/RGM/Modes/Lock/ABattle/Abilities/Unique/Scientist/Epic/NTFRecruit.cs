using MEC;
using PlayerRoles;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scientist.Epic;

[Ability("모집", 
    "즉시 NTF 상등병으로 변경하며, 워크스테이션 이용 기록을 초기화합니다.",
    AbilityCategory.Epic, AbilityType.EPIC_SCIENTIST_NTFRECRUIT, RoleAbility.Scientist)]

public class NTFRecruit : Ability
{
    public override void OnEnabled()
    {
        Vector3 pos = Owner.Position;

        Timing.CallDelayed(0.1f, () =>
        {
            if (!Owner.IsDead) return;
            Owner.Role.Set(RoleTypeId.NtfSpecialist, RoleSpawnFlags.AssignInventory);
            Owner.Position = pos;
            Owner.AddAbility(AbilityType.EPIC_LUCKYVIKEY);
        });
    }
}