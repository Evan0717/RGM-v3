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

namespace DAONTFT.Core.TFT.Euclid.Scp106;

[TFTAbility("잡았다 요놈", "포착ㅣ능력의 쿨타임이 25% 감소합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp106, TFTAbilityPoint.Once, TFTAbilityType.Capture, "📷")]
public class Capture : TFTAbility
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp106Role scp106)
        {
            scp106.CaptureCooldown *= 0.75f;
        }
    }

    public override void OnDisabled()
    {
    }
}
