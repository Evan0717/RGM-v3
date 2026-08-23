using MEC;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.Human.Epic;

[Ability("환생", "즉시 본인 진영의 최초 스폰 지점으로 이동하며, 워크스테이션 이용 기록을 초기화합니다.",
    AbilityCategory.Epic, AbilityType.EPIC_HUMAN_REBIRTH, RoleAbility.Human)]
public class Rebirth : Ability
{
    public override void OnEnabled()
    {
        RoleTypeId roleId = Owner.Role.Type;
        
        Timing.CallDelayed(0.1f, () => 
        {
            Owner.Role.Set(roleId);
            Owner.AddAbility(AbilityType.EPIC_LUCKYVIKEY);
        });

        Timing.CallDelayed(1f, () =>
        {
            Owner.RemoveAbility(this);
            Owner.AddAbility(AbilityType.DUMMY_REBIRTHCOMPLETE);
        });
    }
}