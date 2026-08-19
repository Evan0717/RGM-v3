using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using MEC;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Unique.Scp0492.Legend;

[Ability("성장",
    """
    SCP-079를 제외한 무작위 SCP로 성장하며, 워크스테이션 이용 기록을 초기화합니다.
    성장 전 체력이 해당 SCP의 기본 체력보다 높다면 해당 체력을 그대로 전수받습니다.
    """,
    AbilityCategory.Legend,
    AbilityType.LEGEND_SCP0492_GROWTH,
    RoleAbility.Scp0492)]
public class Growth : Ability
{
    private static readonly List<RoleTypeId> GrowthRoles = Tools.EnumToList<RoleTypeId>()
        .Where(role => role.IsScp() && role is not RoleTypeId.Scp0492 and not RoleTypeId.Scp079)
        .ToList();

    public override void OnEnabled()
    {
        RoleTypeId growthRole = GrowthRoles.GetRandomValue();
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            SetGrowthRole(Owner, growthRole);
            Owner.AddAbility(AbilityType.EPIC_LUCKYVIKEY);
        });
    }
    
    private static void SetGrowthRole(Exiled.API.Features.Player target, RoleTypeId growthRole)
    {
        if (target is not { IsAlive: true } || target.Role.Type != RoleTypeId.Scp0492)
            return;

        float previousHealth = target.Health;
        target.Role.Set(growthRole, RoleSpawnFlags.None);

        if (previousHealth > target.MaxHealth)
            target.Health = previousHealth;
    }
}