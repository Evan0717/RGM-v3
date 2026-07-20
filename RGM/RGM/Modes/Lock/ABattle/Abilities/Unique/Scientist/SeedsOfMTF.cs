using Exiled.API.Features;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.Scientist;

[Ability("구미호의 씨앗", "NTF 스폰 영향력을 20 추가합니다.", AbilityCategory.Common, AbilityType.COMMON_SCIENTIST_SEEDSOFMTF, RoleAbility.Scientist)]
public class SeedsOfMTF : Ability
{
    public override void OnEnabled()
    {
        Respawn.GrantInfluence(Faction.FoundationStaff, 20);
    }

    public override void OnDisabled()
    {
    }
}
