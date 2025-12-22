using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
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

[Ability("휴게소", "[경험치 획득]ㅣ생존한 SCP의 체력이 획득한 경험치의 10%만큼 회복됩니다.", AbilityCategory.Scp079, AbilityType.SCP079_RESTAREA)]
public class RestArea : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.GainingExperience += OnGainingExperience;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.GainingExperience -= OnGainingExperience;
    }

    public void OnGainingExperience(GainingExperienceEventArgs ev)
    {
        if (Owner != ev.Player)
            return;

        foreach (var scp in PlayerManager.List.Where(x => x.IsScpRole()))
        {
            if (scp.Health >= scp.MaxHealth)
                continue;

            scp.Health += ev.Amount * 0.1f;
        }
    }
}
