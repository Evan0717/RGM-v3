using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Normal;

[Ability("보험", "사망 판정을 받을 경우 1번 버텨냅니다.", AbilityCategory.Common, AbilityType.NORMAL_INSURANCE)]
public class Insurance : Ability
{
    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Dying += OnDying;
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Dying -= OnDying;
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (ev.Player != Owner) 
            return;

        ev.IsAllowed = false;
        ev.Player.RemoveAbility(this);

        Owner.ShowHint($"사망 판정을 받았지만 <color={ABattle.RatingColor["일반"]}>보험</color>으로 인해 1번 버텨냅니다.");
    }
}
