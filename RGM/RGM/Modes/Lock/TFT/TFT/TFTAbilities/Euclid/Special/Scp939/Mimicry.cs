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

namespace DAONTFT.Core.TFT.Euclid.Scp939;

[TFTAbility("흉내쟁이", "흉내ㅣ능력의 쿨타임이 75% 감소합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp939, TFTAbilityPoint.Once, TFTAbilityType.Mimicry, "🔊")]
public class Mimicry : TFTAbility
{
    public override void OnEnabled()
    {
        if (Owner.Role is Scp939Role scp939)
        {
            scp939.MimicryCooldown *= 0.25f;
        }
    }

    public override void OnDisabled()
    {
    }
}
