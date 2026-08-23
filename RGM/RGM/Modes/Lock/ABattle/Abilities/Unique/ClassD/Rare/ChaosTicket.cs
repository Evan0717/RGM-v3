using Exiled.API.Features;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.ClassD.Rare;

[Ability("CHAOS 이용권", "CHAOS 진영에 토큰 1개를 추가합니다.", 
    AbilityCategory.Rare, AbilityType.RARE_CLASSD_CHAOSTICKET, RoleAbility.ClassD)]

public class ChaosTicket : Ability
{
    public override void OnEnabled()
    {
        Faction faction = Owner.Role.Team.GetFaction();
        if (faction is not (Faction.FoundationStaff or Faction.FoundationEnemy))
            return;

        Respawn.GrantTokens(faction, 1);
    }
}