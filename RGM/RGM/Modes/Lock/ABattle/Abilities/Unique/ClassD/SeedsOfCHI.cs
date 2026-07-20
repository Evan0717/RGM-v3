using Exiled.API.Features;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.ClassD;

[Ability("반란의 씨앗", "CHI 스폰 영향력을 20 추가합니다.", AbilityCategory.Common, AbilityType.COMMON_CLASSD_SEEDSOFCHI, RoleAbility.ClassD)]
public class SeedsOfCHI : Ability
{
    public override void OnEnabled()
    {
        Respawn.GrantInfluence(Faction.FoundationEnemy, 20);
    }

    public override void OnDisabled()
    {
    }
}
