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

namespace DAONTFT.Core.TFT.Euclid.Scp079;

[TFTAbility("폐소공포증", "폐쇄ㅣ능력의 쿨타임이 25% 감소합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp079, TFTAbilityPoint.Once, TFTAbilityType.RoomLockdown, "🔒")]
public class RoomLockdown : TFTAbility
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp079Role scp079)
        {
            scp079.RoomLockdownCooldown *= 0.75f;
        }
    }

    public override void OnDisabled()
    {
    }
}
