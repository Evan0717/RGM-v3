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

[TFTAbility("반블럭", "공격에 맞으면 이동 속도가 2초 동안 1% 증가합니다. (중첩 가능)", TFTAbilityLevel.Euclid, TFTAbilityCategory.Scp3114, TFTAbilityPoint.Continuous, TFTAbilityType.Half, "📘")]
public class Half : TFTAbility
{
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
        if (ev.Attacker != null && Owner == ev.Player)
        {
            Owner.AddEffect(EffectType.MovementBoost, 1, 2);
        }
    }
}
