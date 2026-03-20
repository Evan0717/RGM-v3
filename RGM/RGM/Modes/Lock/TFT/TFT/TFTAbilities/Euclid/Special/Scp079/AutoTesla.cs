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

namespace DAONTFT.Core.TFT.Euclid.Scp079;

[TFTAbility("자동 방어 시스템", "테슬라가 가끔씩 자동으로 작동됩니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp079, TFTAbilityPoint.Continuous, TFTAbilityType.AutoTesla, "📡")]
public class AutoTesla : TFTAbility
{
    CoroutineHandle _autoTesla;

    public override void OnEnabled()
    {
        _autoTesla = Timing.RunCoroutine(autoTesla());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_autoTesla);
    }

    IEnumerator<float> autoTesla()
    {
        while (true)
        {
            LabApi.Features.Wrappers.Tesla tesla = LabApi.Features.Wrappers.Tesla.List.GetRandomValue();
            tesla.Trigger();
            tesla.InstantTrigger();

            yield return Timing.WaitForSeconds(Random.Range(1, 60));
        }
    }
}
