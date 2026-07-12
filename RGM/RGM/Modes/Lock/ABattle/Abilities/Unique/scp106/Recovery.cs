using Exiled.API.Features.Roles;

namespace RGM.Modes.Abilities.Unique.Scp106;

[Ability("회춘", "공격 쿨타임이 20% 줄어듭니다.", AbilityCategory.Scp106, AbilityType.SCP106_RECOVERY)]
public class Recovery : Ability
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp106Role scp106) 
            scp106.CaptureCooldown *= 0.8f;
    }

    public override void OnDisabled()
    {
    }
}
