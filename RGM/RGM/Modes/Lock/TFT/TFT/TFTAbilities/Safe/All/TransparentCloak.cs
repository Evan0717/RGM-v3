using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Safe.All;

[TFTAbility("투명 망토", "25초간 투명해집니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.TransparentCloak, "🪞")]
public class TransparentCloak : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.EnableEffect(EffectType.Invisible, 1, 25);
    }

    public override void OnDisabled()
    {
    }
}
