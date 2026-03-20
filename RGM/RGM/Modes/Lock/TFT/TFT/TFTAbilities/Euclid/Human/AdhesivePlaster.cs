using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Extension;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using static DAONTFT.Core.Variables.Base;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("반창고", "체력이 절반 이하로 줄어들었을 경우 120만큼 즉시 회복합니다. (최대 체력 무시)", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Continuous, TFTAbilityType.AdhesivePlaster, "😙")]
public class AdhesivePlaster : TFTAbility
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (ev.Player.Health <= ev.Player.MaxHealth / 2)
        {
            ev.Player.Health += 120;

            Data.Description = "체력이 절반 이하로 줄어들었을 경우 120만큼 즉시 회복합니다. (사용 완료)";

            OnDisabled();
        }
    }
}
