using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

namespace DAONTFT.Core.TFT.Safe.Scp079;

[TFTAbility("간이 충전기", "즉시 20의 경험치를 획득합니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.Scp079, TFTAbilityPoint.Once, TFTAbilityType.Charger, "✨")]
public class Charger : TFTAbility
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp079Role scp079Role)
        {
            scp079Role.AddExperience(20);
        }
    }

    public override void OnDisabled()
    {
    }
}
