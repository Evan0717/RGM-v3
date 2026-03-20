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

[TFTAbility("종아리Ⅲ", "32%만큼 더 높이 점프할 수 있습니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddJump3, "🐎")]
public class AddJump3 : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.Lightweight, 32);
    }

    public override void OnDisabled()
    {
    }
}
