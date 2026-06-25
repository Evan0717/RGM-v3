using System.Collections.Generic;
using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("꿈나무+", "자신의 진영의 지원 영향력을 16 추가합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.Seed2, "🌳")]
public class Seed2 : TFTAbility
{
    public override void OnEnabled()
    {
        Respawn.GrantInfluence(Owner.Role.Team.GetFaction(), 16);
    }

    public override void OnDisabled()
    {
    }
}
