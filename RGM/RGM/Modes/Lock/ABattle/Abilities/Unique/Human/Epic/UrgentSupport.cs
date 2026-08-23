using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using Respawning;

namespace RGM.Modes.Abilities.Unique.Human.Epic;

[Ability("긴급 지원", "즉시 본인 진영의 정규 지원을 소환합니다. 해당 지원은 지원 토큰을 소모하지 않습니다.",
    AbilityCategory.Epic, AbilityType.EPIC_HUMAN_URGENTSUPPORT, RoleAbility.Human)]
public class UrgentSupport : Ability
{
    public override void OnEnabled()
    {
        if (Owner.Role.Type == RoleTypeId.Tutorial)
        {
            EventArgs.ServerEvents.CallTutorialSupport(Player.List.Where(player => player.IsDead));
            return;
        }

        Faction faction = Owner.Role.Team.GetFaction();
        if (faction is not (Faction.FoundationStaff or Faction.FoundationEnemy))
            return;

        Respawn.GrantTokens(faction, 1);

        if (WaveManager.TryGet(faction, out var wave))
            WaveManager.Spawn(wave);
    }
}