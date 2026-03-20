using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

using static DAONTFT.Core.Variables.Base;

namespace DAONTFT.Core.TFT.Euclid.All;

[TFTAbility("빠른 판단", "1분 동안 무적 상태(일부 데미지 제외)가 되며, 체포되지 않습니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.QuickJudgement, "💦")]
public class QuickJudgement : TFTAbility
{
    public override void OnEnabled()
    {
        GodModePlayers.Add(Owner);

        Timing.CallDelayed(60, () =>
        {
            GodModePlayers.Remove(Owner);
        });

        Owner.Cuffer = null;

        Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Handcuffing -= OnHandcuffing;
    }

    void OnHandcuffing(HandcuffingEventArgs ev)
    {
        if (ev.Target == Owner)
            ev.IsAllowed = false;
    }
}
