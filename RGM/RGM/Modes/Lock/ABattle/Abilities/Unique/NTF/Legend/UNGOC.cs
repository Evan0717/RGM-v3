using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Enums;
using MEC;
using PlayerRoles;

namespace RGM.Modes.Abilities.Unique.NTF.Legend;

[Ability("U.N.G.O.C", 
    """
    SCP 전문 파괴 집단, U.N.G.O.C 지원을 호출합니다.
    U.N.G.O.C 대원은 기본적으로 강화된 능력치를 가집니다.
    """,
    AbilityCategory.Legend, AbilityType.LEGEND_NTF_UNGOC, RoleAbility.NTF)]

public class UNGOC : Ability
{
    private const float RoleChangeDelay = 0.1f;
    private CoroutineHandle _summonCoroutine;

    public override void OnEnabled()
    {
        _summonCoroutine = Timing.RunCoroutine(SummonGocMembers());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_summonCoroutine);
    }

    private static IEnumerator<float> SummonGocMembers()
    {
        List<Player> targets = Player.List.Where(player => player.IsDead).ToList();

        foreach (Player target in targets)
        {
            if (!target.IsDead)
                continue;

            // Chaos Repressor의 장비를 먼저 지급하고, NTF Captain으로 전환할 때는 장비를 재지급하지 않는다.
            target.Role.Set(RoleTypeId.ChaosRepressor, SpawnReason.ForceClass, RoleSpawnFlags.AssignInventory);
            yield return Timing.WaitForSeconds(RoleChangeDelay);

            if (!target.IsAlive || target.Role.Type != RoleTypeId.ChaosRepressor)
                continue;

            target.Role.Set(RoleTypeId.NtfCaptain, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);
            yield return Timing.WaitForSeconds(RoleChangeDelay);

            if (target.IsAlive && target.Role.Type == RoleTypeId.NtfCaptain)
                target.AddAbility(AbilityType.DUMMY_GOCMEMBER);
        }
    }
}