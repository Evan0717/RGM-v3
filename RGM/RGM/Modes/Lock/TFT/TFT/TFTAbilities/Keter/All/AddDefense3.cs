using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Keter.All;

[TFTAbility("방어Ⅲ", "방어력을 얻습니다. (+40%)", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddDefense3, "⛔")]
public class AddDefense3 : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.DamageReduction, 80);
    }

    public override void OnDisabled()
    {
    }
}
