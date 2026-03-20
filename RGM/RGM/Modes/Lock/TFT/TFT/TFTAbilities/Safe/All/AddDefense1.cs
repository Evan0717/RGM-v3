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

namespace DAONTFT.Core.TFT.Safe.All;

[TFTAbility("방어Ⅰ", "방어력을 얻습니다. (+10%)", TFTAbilityLevel.Safe, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddDefense1, "⛔")]
public class AddDefense1 : TFTAbility
{
    public override void OnEnabled()
    {
        Owner.AddEffect(EffectType.DamageReduction, 20);
    }

    public override void OnDisabled()
    {
    }
}
