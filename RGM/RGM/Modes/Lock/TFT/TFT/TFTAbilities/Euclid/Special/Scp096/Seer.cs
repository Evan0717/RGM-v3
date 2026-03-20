using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using HintServiceMeow.Core.Extension;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Euclid.Scp096;

[TFTAbility("천리안", "분노 시 인근에 있는 인간들을 목격자에 포함시킵니다. (최대 3명)", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp096, TFTAbilityPoint.Continuous, TFTAbilityType.Seer, "🔭")]
public class Seer : TFTAbility
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp096.Enraging += OnEnraging;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp096.Enraging -= OnEnraging;
    }

    public void OnEnraging(EnragingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        int Stack = 0;

        foreach (var player in Player.List.Where(x => x.IsHuman))
        {
            if (Stack == 3)
                break;

            if (Vector3.Distance(player.Position, ev.Player.Position) < 21)
            {
                Stack += 1;

                ev.Scp096.AddTarget(player);
            }
        }
    }
}
