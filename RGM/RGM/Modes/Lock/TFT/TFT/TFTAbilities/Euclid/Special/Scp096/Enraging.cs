using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp096;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

namespace DAONTFT.Core.TFT.Euclid.Scp096;

[TFTAbility("지구력", "돌격ㅣ능력의 지속 시간이 50% 증가합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp096, TFTAbilityPoint.Continuous, TFTAbilityType.Enraging, "📶")]
public class Enraging : TFTAbility
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp096.Enraging += OnEnraging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp096.Enraging -= OnEnraging;
    }

    void OnEnraging(EnragingEventArgs ev)
    {
        ev.InitialDuration *= 1.5f;
    }
}
