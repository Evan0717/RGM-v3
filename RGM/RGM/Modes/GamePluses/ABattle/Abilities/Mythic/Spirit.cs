using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.EventArgs;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace RGM.Modes.Abilities.Mythic;

[Ability("스피릿", "영혼 상태로 상시 전환됩니다!", AbilityCategory.Mythic, AbilityType.MYTHIC_SPIRIT)]
public class Spirit : Ability
{
    CoroutineHandle _onStarted;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Hurt += OnHurt;

        _onStarted = Timing.RunCoroutine(OnStarted());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Hurt -= OnHurt;

        Timing.KillCoroutines(_onStarted);
    }

    public IEnumerator<float> OnStarted()
    {
        while (true)
        {
            Owner.EnableEffect(EffectType.Invisible);

            yield return Timing.WaitForSeconds(2f);
        }
    }

    public void OnHurt(HurtEventArgs ev)
    {
        if (ev.Attacker == Owner)
            ev.Attacker.DisableEffect(EffectType.Invisible);

        if (ev.Player == Owner)
            ev.Player.EnableEffect(EffectType.Invisible);
    }
}