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

namespace DAONTFT.Core.TFT.Safe.Human;

[TFTAbility("소인화", "몸의 크기가 10% 줄어듭니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.Miniaturization, "👤")]
public class Miniaturization : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.Scale = new Vector3(0.9f, 0.9f, 0.9f);
    }

    public override void OnDisabled()
    {
    }
}
