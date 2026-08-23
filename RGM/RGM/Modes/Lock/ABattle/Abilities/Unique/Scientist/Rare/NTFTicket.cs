using Exiled.API.Features;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.Scientist.Rare;

[Ability("NTF 이용권", "NTF 진영에 토큰 1개를 추가합니다.", 
    AbilityCategory.Rare, AbilityType.RARE_SCIENTIST_NTFTICKET, RoleAbility.Scientist)]

public class NTFTicket : Ability
{
    public override void OnEnabled()
    {
        Faction faction = Owner.Role.Team.GetFaction();
        if (faction is not (Faction.FoundationStaff or Faction.FoundationEnemy))
            return;

        Respawn.GrantTokens(faction, 1);
    }
}