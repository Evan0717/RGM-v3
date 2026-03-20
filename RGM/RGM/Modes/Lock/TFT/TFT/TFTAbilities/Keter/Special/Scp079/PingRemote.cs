using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Keter.Scp079;

[TFTAbility("휴대용 정전기", "핑ㅣ해당 방이 2.5초 간 정전이 됩니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Scp079, TFTAbilityPoint.Continuous, TFTAbilityType.PingRemote, "🔲")]
public class PingRemote : TFTAbility
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging += OnPinging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp079.Pinging -= OnPinging;
    }

    public void OnPinging(PingingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (!ev.Room.AreLightsOff)
            ev.Room.TurnOffLights(2.5f);
    }
}
