using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Extension;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using static DAONTFT.Core.Variables.Base;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("무기 전문가", "1분 후, 녹즙을 획득합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.Scp1853, "🔫")]
public class Scp1853 : TFTAbility
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(60, () =>
        {
            Owner.AddItem(ItemType.SCP1853);
        });
    }

    public override void OnDisabled()
    {
    }
}
