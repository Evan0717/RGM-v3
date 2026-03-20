using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

namespace DAONTFT.Core.TFT.Euclid.Scp049;

[TFTAbility("심부름", "의사의 부름ㅣ능력의 쿨타임이 25% 감소합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp049, TFTAbilityPoint.Once, TFTAbilityType.Call, "👓")]
public class Call : TFTAbility
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp049Role scp049)
        {
            scp049.CallCooldown *= 0.75f;
        }
    }

    public override void OnDisabled()
    {
    }
}
