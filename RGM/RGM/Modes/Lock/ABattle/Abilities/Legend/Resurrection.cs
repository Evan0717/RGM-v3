using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Legend;

[Ability("리저렉션",
    """
    죽은 아군 전체를 되살립니다. 자신의 진영이 SCP인 경우, 관전자 중 1명을 079와 049-2를 제외한 랜덤한 SCP로 부활시킵니다.
    추가로, 부활한 대상에게 랜덤으로 15개 능력을 부여합니다.
    """,
    AbilityCategory.Legend, AbilityType.LEGEND_RESURRECTION)]

public class Resurrection : Ability
{
    private const float RoleChangeDelay = 0.1f;

    private static readonly List<RoleTypeId> RandomScpRoles =
    [
        .. Tools.EnumToList<RoleTypeId>()
            .Where(role => role.IsScp() && role is not RoleTypeId.Scp0492 and not RoleTypeId.Scp079)
    ];

    private static readonly List<AbilityType> ExceptAbilities =
    [
        AbilityType.LEGEND_RESURRECTION, AbilityType.EPIC_PRIEST, AbilityType.LEGEND_CATACLYSMGENERATOR, 
        AbilityType.LEGEND_REPLICATION, AbilityType.NORMAL_TEST, AbilityType.RARE_FINALEXAM, 
        AbilityType.EPIC_CSAT, AbilityType.NORMAL_REROLL, AbilityType.NORMAL_EXCHANGE,
        AbilityType.NORMAL_SUSPICIOUSSTEW, AbilityType.RARE_DND, AbilityType.RARE_TELEPORTATION, 
        AbilityType.RARE_UPGRADE, AbilityType.RARE_CONTRACT
    ];

    public override void OnEnabled()
    {
        Timing.RunCoroutine(Resurrect());
    }

    private IEnumerator<float> Resurrect()
    {
        List<Player> deadPlayers = PlayerManager.List.Where(player => player.IsDead).ToList();
        List<Player> reviveTargets = Owner.IsScpRole()
            ? deadPlayers.Count > 0 ? [deadPlayers.GetRandomValue()] : []
            : deadPlayers.Where(player =>
                ABattle.Instance.LastDeathRoles.TryGetValue(player, out RoleTypeId deathRole) &&
                RoleExtensions.GetTeam(deathRole) == RoleExtensions.GetTeam(Owner.Role.Type))
                .ToList();

        foreach (Player target in reviveTargets)
        {
            if (!target.IsDead)
                continue;

            RoleTypeId role = Owner.IsScpRole()
                ? RandomScpRoles.GetRandomValue()
                : Owner.Role.Type;

            target.Role.Set(role, RoleSpawnFlags.None);
            yield return Timing.WaitForSeconds(RoleChangeDelay);

            if (!target.IsAlive || target.Role.Type != role)
                continue;

            target.Position = Owner.Position;

            for (int i = 0; i < 15; i++)
            {
                AbilityType ability = ABattle.Instance.GetRandomAbilities(
                    target,
                    ABattle.GetCategory(target, allowAncient: false),
                    1,
                    ExceptAbilities).First();
                ABattle.Instance.AddAbility(target, ability, allowReflector: false);
            }
        }
    }
}