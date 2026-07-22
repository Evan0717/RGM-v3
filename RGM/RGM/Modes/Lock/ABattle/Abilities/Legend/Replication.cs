using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;

namespace RGM.Modes.Abilities.Legend;

[Ability("복제", $"가지고 있는 능력의 개수를 2배로 증가시킵니다.", AbilityCategory.Legend, AbilityType.LEGEND_REPLICATION, RoleAbility.None, true)]
public class Replication : Ability
{
    public override void OnEnabled()
    {
        foreach (var ability in Owner.GetAbilities().Where(a => a.Data.AbilityType != AbilityType.LEGEND_REPLICATION).ToList())
        {
            Owner.AddAbility(ability.Data.AbilityType);
        }
    }

    public override void OnDisabled()
    {
    }
}
