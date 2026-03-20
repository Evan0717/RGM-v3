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

[TFTAbility("잠수부", "시야가 개선되고 스태미나가 무제한이 됩니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Continuous, TFTAbilityType.Diver, "🫁")]
public class Diver : TFTAbility
{
    CoroutineHandle _diverRotation;

    public override void OnEnabled()
    {
        _diverRotation = Timing.RunCoroutine(DiverRotation());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_diverRotation);

        Owner.DisableEffect(EffectType.Invigorated);
    }

    public IEnumerator<float> DiverRotation()
    {
        while (true)
        {
            Owner.EnableEffect(EffectType.Invigorated);

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
