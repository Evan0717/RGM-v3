using System.Collections.Generic;
using Exiled.API.Features.Roles;
using MEC;
namespace RGM.Modes.Abilities.Unique.Scp079.Epic;

[Ability("만렙", "즉시 5티어까지 상승합니다.", AbilityCategory.Epic, AbilityType.EPIC_SCP079_LEVELUP, RoleAbility.Scp079)]
public class LevelUp : Ability
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp079Role scp079)
            scp079.TierManager.AccessTierIndex = 4;

    }

    public override void OnDisabled()
    {

    }

}
