using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp049;

[Ability("실험체", "'[일반] 보험' 능력을 획득하면 '[영웅] 매드 사이언티스트' 능력을 획득합니다.", AbilityCategory.Scp049, AbilityType.SCP049_MADDOCTOR)]
public class MadDoctor : Ability
{
    CoroutineHandle _mad;

    public override void OnEnabled()
    {
        _mad = Timing.RunCoroutine(mad());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_mad);
    }

    IEnumerator<float> mad()
    {
        while (true)
        {
            if (Owner.HasAbility(AbilityType.NORMAL_INSURANCE))
            {
                Owner.AddAbility(AbilityType.EPIC_MADSCIENTIST);

                OnDisabled();
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
