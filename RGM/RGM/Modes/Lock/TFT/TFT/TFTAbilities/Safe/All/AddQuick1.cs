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

namespace DAONTFT.Core.TFT.Safe.All;

[TFTAbility("순발력Ⅰ", "5% 확률로 공격을 회피합니다.", TFTAbilityLevel.Safe, TFTAbilityCategory.All, TFTAbilityPoint.Continuous, TFTAbilityType.AddQuick1, "🍃")]
public class AddQuick1 : TFTAbility
{
    int count = 0;
    float damage = 0;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
    }

    void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Player == Owner)
        {
            if (Random.Range(0, 20) == 0)
            {
                ev.IsAllowed = false;
                count++;
                damage += ev.Amount;

                Data.Description = $"5% 확률로 공격을 회피합니다. (피한 횟수: {count}, 피한 데미지: {(int)damage})";
            }
        }
    }
}
