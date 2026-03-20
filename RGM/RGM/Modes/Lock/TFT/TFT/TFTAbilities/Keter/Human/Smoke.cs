using CustomPlayerEffects;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Extension;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DAONTFT.Core.TFT.Keter.Human;

[TFTAbility("연막술사", "튀니지식 토기 항아리를 4개 획득합니다.", TFTAbilityLevel.Keter, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.Smoke, "🎳")]
public class Smoke : TFTAbility
{
    public override void OnEnabled()
    {
        for (int i = 0; i < 2; i++)
        {
            Owner.AddItem(ItemType.SCP244a);
        }

        for (int i = 0; i < 2; i++)
        {
            Owner.AddItem(ItemType.SCP244b);
        }
    }

    public override void OnDisabled()
    {
    }
}
