using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Scp106;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Unique.Scp079;

[Ability("과전류", "1분 간 전력이 무제한이 됩니다.", AbilityCategory.Scp079, AbilityType.SCP079_OVERCURRENT)]
public class OverCurrent : Ability
{
    CoroutineHandle _onStarted;

    public override void OnEnabled()
    {
        _onStarted = Timing.RunCoroutine(OnStarted());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_onStarted);
    }

    public IEnumerator<float> OnStarted()
    {
        for (int i = 1; i < 61; i++)
        {
            if (Owner.Role is Scp079Role scp0791)
                scp0791.Energy = scp0791.MaxEnergy;

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
