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

[TFTAbility("초재생", "1분 후, 안티 콜라를 획득합니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.AntiScp207, "🍹")]
public class AntiScp207 : TFTAbility
{
    public override void OnEnabled()
    {
        Timing.CallDelayed(60, () =>
        {
            Owner.AddItem(ItemType.AntiSCP207);
        });
    }

    public override void OnDisabled()
    {
    }
}
