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
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

namespace DAONTFT.Core.TFT.Keter.Scp049;

[TFTAbility("명의", "시체를 되살리기 시작한 후, 3초 후에도 시체가 만료되지 않았다면 즉시 되살립니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Scp049, TFTAbilityPoint.Continuous, TFTAbilityType.BestDoctor, "💉")]
public class BestDoctor : TFTAbility
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Scp049.StartingRecall += OnStartingRecall;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Scp049.StartingRecall -= OnStartingRecall;
    }

    void OnStartingRecall(StartingRecallEventArgs ev)
    {
        Timing.CallDelayed(3, () =>
        {
            if (ev.Scp049.CanResurrect(ev.Ragdoll) && ev.Scp049.IsInRecallRange(ev.Ragdoll))
            {
                ev.Scp049.Resurrect(ev.Player);
            }
        });
    }
}
