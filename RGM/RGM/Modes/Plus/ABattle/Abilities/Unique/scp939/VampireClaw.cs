using Exiled.API.Features.Roles;

namespace RGM.Modes.Abilities.Unique.Scp939;

[Ability("흡혈 발톱", "공격 시 36의 HS가 회복됩니다.", AbilityCategory.Scp939, AbilityType.SCP939_VAMPIRECLAW)]
public class VampireClaw : Ability
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp939Role Scp939)
            Scp939.Owner.HumeShield += 36;
    }

    public override void OnDisabled()
    {
    }
}
