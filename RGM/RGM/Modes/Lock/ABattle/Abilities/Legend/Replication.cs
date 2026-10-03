using System.Collections.Generic;
using System.Linq;
using MEC;

namespace RGM.Modes.Abilities.Legend;

[Ability("복제", $"가지고 있는 능력의 개수를 2배로 증가시킵니다.",
    AbilityCategory.Legend, AbilityType.LEGEND_REPLICATION, RoleAbility.None, true)]
public class Replication : Ability
{
    public override void OnEnabled()
    {
        Owner.GetAbilities().Where(a =>
                a.Data.Category != AbilityCategory.Ancient &&
                a.Data.AbilityType != AbilityType.LEGEND_REPLICATION &&
                a.Data.AbilityType != AbilityType.LEGEND_CATACLYSMGENERATOR).ToList()
            .ForEach(x => Timing.RunCoroutine(ABattle.Instance.AddAbilityCoroutine(Owner, 
                [x.Data.AbilityType], allowReflector: false)));
    }
}