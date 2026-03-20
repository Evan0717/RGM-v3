using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Classes;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace DAONTFT.Core.TFT.Safe.Human;

[TFTAbility("트릭 오어 트릿", "무작위 사탕을 받습니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.TrickOrTreat, "🍬")]
public class TrickorTreat : TFTAbility
{
    public override void OnEnabled()
    {
        if (Random.Range(1, 101) == 1)
        {
            Owner.AddCandy(CandyKindID.Pink);
        }
        else
        {
            Owner.AddRandomCandy();
        }
    }

    public override void OnDisabled()
    {
    }
}
