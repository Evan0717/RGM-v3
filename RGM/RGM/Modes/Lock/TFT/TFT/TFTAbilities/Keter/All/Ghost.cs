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

namespace DAONTFT.Core.TFT.Keter.All;

[TFTAbility("유령화", "아이템을 전부 버린 상태라면 문을 뚫고 다닐 수 있습니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Continuous, TFTAbilityType.Ghost, "👻")]
public class Ghost : TFTAbility
{
    CoroutineHandle _ghostLoop;
    public override void OnEnabled()
    {
        _ghostLoop = Timing.RunCoroutine(ghostLoop());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_ghostLoop);
    }

    IEnumerator<float> ghostLoop()
    {
        while (true)
        {
            if (Owner.Items.Count(x => !x.IsAmmo) == 0)
            {
                Owner.AddEffect(EffectType.Ghostly, 1, 1.1f);
            }

            yield return Timing.WaitForSeconds(1);
        }
    }
}
