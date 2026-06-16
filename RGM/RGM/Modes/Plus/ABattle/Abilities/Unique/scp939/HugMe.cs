using Exiled.API.Features.Roles;

namespace RGM.Modes.Abilities.Unique.Scp939;

[Ability("안아줘요", "안개 쿨타임이 50% 줄어듭니다.", AbilityCategory.Scp939, AbilityType.SCP939_HUGME)]
public class HugMe : Ability
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp939Role Scp939)
            Scp939.AmnesticCloudCooldown *= 0.5f;
    }

    public override void OnDisabled()
    {
    }
}
