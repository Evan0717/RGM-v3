using System.Collections.Generic;
using System.Linq;
using MEC;

namespace RGM.Modes.Abilities.Legend;

/*
[Ability("복제", $"가지고 있는 능력의 개수를 2배로 증가시킵니다.",
    AbilityCategory.Legend, AbilityType.LEGEND_REPLICATION, RoleAbility.None, true)]*/
public class Replication : Ability
{
    public override void OnEnabled()
    {
        Timing.RunCoroutine(Do());
    }

    private IEnumerator<float> Do()
    {
        foreach (var ability in Owner.GetAbilities().Where(a =>
                     a.Data.Category != AbilityCategory.Ancient &&
                     a.Data.AbilityType != AbilityType.LEGEND_REPLICATION &&
                     a.Data.AbilityType != AbilityType.LEGEND_CATACLYSMGENERATOR).ToList())
        {
            // 복제로 지급되는 능력에는 반사경 연쇄가 발동되지 않음
            ABattle.Instance.AddAbility(Owner, ability.Data.AbilityType, allowReflector: false);
            yield return Timing.WaitForOneFrame;
        }

        yield break;
    }
}
