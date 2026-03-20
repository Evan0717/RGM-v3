using System.Collections.Generic;
using System.Linq;
using DAONTFT.Core.Functions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using UnityEngine;

using static DAONTFT.Core.Variables.Base;

namespace DAONTFT.Core.TFT.Euclid.Human;

[TFTAbility("탄약고", "모든 종류의 탄약을 각각 5세트씩 받습니다.", TFTAbilityLevel.Euclid, TFTAbilityCategory.Human, TFTAbilityPoint.Once, TFTAbilityType.AmmoBox, "🔫")]
public class CandyAddict : TFTAbility
{
    public override void OnEnabled()
    {
        foreach (var ammo in Function.EnumToList<ItemType>().Where(x => x.IsAmmo()))
        {
            for (int i = 0; i < 5; i++)
            {
                Owner.AddItem(ammo);
            }
        }
    }

    public override void OnDisabled()
    {
    }
}
