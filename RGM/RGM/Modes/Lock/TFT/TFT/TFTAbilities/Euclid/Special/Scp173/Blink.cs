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

namespace DAONTFT.Core.TFT.Euclid.Scp173;

[TFTAbility("눈 깜빡할 사이에", "깜빡ㅣ능력의 쿨타임이 25% 감소합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp173, TFTAbilityPoint.Once, TFTAbilityType.Blink, "👀")]
public class Blink : TFTAbility
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp173Role scp173)
        {
            scp173.BlinkCooldown *= 0.75f;
        }
    }

    public override void OnDisabled()
    {
    }
}
