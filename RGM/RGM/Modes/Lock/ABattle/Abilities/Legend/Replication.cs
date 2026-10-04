using System.Collections.Generic;
using System.Linq;

namespace RGM.Modes.Abilities.Legend;

[Ability("복제", $"가지고 있는 능력의 개수를 2배로 증가시킵니다.",
    AbilityCategory.Legend, AbilityType.LEGEND_REPLICATION, RoleAbility.None, true, isUnique: true)]

public class Replication : Ability
{
    public override void OnEnabled()
    {
        List<AbilityType> types = [];
        Owner.GetAbilities().Where(a =>
                a.Data.Category != AbilityCategory.Ancient &&
                a.Data.AbilityType != AbilityType.LEGEND_REPLICATION &&
                a.Data.AbilityType != AbilityType.LEGEND_CATACLYSMGENERATOR).ToList()
            .ForEach(x => types.Add(x.Data.AbilityType));


        _ = ABattle.Instance.AddAbilityAsync(Owner, types, allowReflector: false);
    }
}