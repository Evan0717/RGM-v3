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

namespace DAONTFT.Core.TFT.Euclid.Scp3114;

[TFTAbility("변장술사", "변장ㅣ능력의 지속 시간이 25% 증가합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp3114, TFTAbilityPoint.Once, TFTAbilityType.Disguise, "🎭")]
public class Disguise : TFTAbility
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp3114Role scp3114)
        {
            scp3114.DisguiseDuration *= 1.25f;
        }
    }

    public override void OnDisabled()
    {
    }
}
