using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.Patches.Events.Player;
using LabApi.Features.Wrappers;
using MEC;
using ProjectMER.Features.Serializable;
using RGM.API.Features;
using RGM.Modes;
using System.Collections.Generic;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Unique.Scp079.Legend;


[Ability("폭발은 예술이다", $"30초마다 <color=#2ECCFA>[희귀]</color> 폭격 능력을 1 ~ 2개 획득합니다.", AbilityCategory.Legend, AbilityType.LEGEND_SCP079_EXPLOSIONISART, RoleAbility.Scp079)]
public class ExplosionIsArt : Ability
{
    CoroutineHandle _airstrike;
    public override void OnEnabled()
    {
        _airstrike = Timing.RunCoroutine(AirstrikeCoroutine());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_airstrike);

    }

    public IEnumerator<float> AirstrikeCoroutine()
    {
        while (Owner.IsAlive && Owner != null)
        {
            if (Random.Range(1, 3) == 1)
            {
                Owner.AddAbility(AbilityType.RARE_SCP079_AIRSTRIKE);
            }
            Owner.AddAbility(AbilityType.RARE_SCP079_AIRSTRIKE);
            yield return Timing.WaitForSeconds(30f);
        }
        yield break;
    }
}