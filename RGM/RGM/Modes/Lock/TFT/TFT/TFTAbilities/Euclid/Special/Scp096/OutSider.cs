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

namespace DAONTFT.Core.TFT.Euclid.Scp096;

[TFTAbility("아웃사이더", "전신이 50% 투명해집니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp096, TFTAbilityPoint.Once, TFTAbilityType.OutSider, "🚫")]
public class OutSider : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.Fade, 50);
    }

    public override void OnDisabled()
    {
    }
}
