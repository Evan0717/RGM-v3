using DAONTFT.Core.Classes;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using static DAONTFT.Core.Variables.Base;

namespace DAONTFT.Core.TFT.Keter.Scp;

[TFTAbility("실험체", "크기가 50% 작아집니다. 다른 실험체와 가까이 있으면 초당 2의 체력을 회복합니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Scp, TFTAbilityPoint.Continuous, TFTAbilityType.TestSubject, "😡")]
public class TestSubject : TFTAbility
{
    CoroutineHandle _test;

    public override void OnEnabled()
    {
        Owner.Scale = new Vector3(0.5f, 0.5f, 0.5f);

        _test = Timing.RunCoroutine(test());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_test);
    }

    IEnumerator<float> test()
    {
        while (true)
        {
            if (Owner.TryGetNearestPlayer(out Player player, out float radius))
            {
                if (player.HasTFTAbility(TFTAbilityType.TestSubject))
                {
                    Owner.Heal(2);
                }
            }

            yield return Timing.WaitForSeconds(1);
        }
    }
}
