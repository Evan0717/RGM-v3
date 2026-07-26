using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;

namespace RGM.Modes.Abilities.Unique.Scp079.Rare;

[Ability("자유", "모든 발전기를 초기화 하고 발전기 가동시간을 1분 늘립니다.", AbilityCategory.Rare, AbilityType.RARE_SCP079_FREEDOM, RoleAbility.Scp079)]
public class Freedom : Ability
{
    public override void OnEnabled()
    {
       foreach (var generator in Generator.List)
       {
         generator.IsEngaged = false;
         generator.IsActivating = false;
         generator.CurrentTime = 0;

         generator.ActivationTime += 60f;
       }
       
    }

    public override void OnDisabled()
    {
    }
}
