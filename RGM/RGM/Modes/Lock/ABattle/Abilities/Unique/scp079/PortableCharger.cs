using Exiled.API.Features.Roles;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("간이 충전기", "즉시 <b>현재 레벨 x 20</b>만큼 경험치를 받습니다.", AbilityCategory.Common, AbilityType.COMMON_SCP079_PORTABLECHARGER, RoleAbility.Scp079)]
public class PortableCharger : Ability
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp079Role scp079)
            scp079.AddExperience(20 * scp079.Level);
    }

    public override void OnDisabled()
    {
    }
}
