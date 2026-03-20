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

[TFTAbility("운동Ⅲ", "최대 체력 + 체력 -> +100❤️ (SCP x10)", TFTAbilityLevel.Keter, TFTAbilityCategory.All, TFTAbilityPoint.Once, TFTAbilityType.AddHealth3, "❤️")]
public class AddHealth3 : TFTAbility
{
    public override void OnEnabled()
    {
        float health = Owner.IsScp ? 1000 : 100;
        Owner.MaxHealth += health;
        Owner.Health += health;
    }
    
    public override void OnDisabled()
    {
    }
}
