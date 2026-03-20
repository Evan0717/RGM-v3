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

namespace DAONTFT.Core.TFT.Euclid.Scp096;

[TFTAbility("분노 조절 문제", "격노ㅣ능력의 쿨타임이 50% 감소합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp096, TFTAbilityPoint.Once, TFTAbilityType.Enrage, "😠")]
public class Enrage : TFTAbility
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp096Role scp096)
        {
            scp096.EnrageCooldown *= 0.5f;
        }
    }

    public override void OnDisabled()
    {
    }
}
