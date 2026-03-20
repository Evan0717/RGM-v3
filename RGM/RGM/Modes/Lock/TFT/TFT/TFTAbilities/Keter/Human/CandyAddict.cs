using DAONTFT.Core.Classes;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DAONTFT.Core.Variables.Base;

namespace DAONTFT.Core.TFT.Keter.Human;

[TFTAbility("마약 중독자", "1분마다 랜덤한 사탕을 획득합니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Human, TFTAbilityPoint.Continuous, TFTAbilityType.CandyAddict, "🍭")]
public class CandyAddict : TFTAbility
{
    CoroutineHandle _candyParty;

    public override void OnEnabled()
    {
        _candyParty = Timing.RunCoroutine(candyParty());
    }

    public override void OnDisabled()
    {
        Timing.KillCoroutines(_candyParty);
    }

    IEnumerator<float> candyParty()
    {
        Owner.AddCandy(Function.EnumToList<CandyKindID>().GetRandomValue());

        while (true)
        {
            if (Owner.IsAlive)
                Owner.AddCandy(Function.EnumToList<CandyKindID>().GetRandomValue());

            for (int i = 0; i < 60; i++)
            {
                Data.Description = $"다음 사탕 획득까지: {60 - i}초";

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
