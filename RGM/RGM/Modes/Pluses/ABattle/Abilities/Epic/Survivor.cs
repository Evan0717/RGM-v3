using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.DataBases;
using RGM.API.Features;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes.Abilities.Epic;

[Ability("구사일생", "사망 판정을 받을 경우, 3초간 투명 상태와 무적이 됩니다. (최대 3번)", AbilityCategory.Epic, AbilityType.EPIC_SURVIVOR)]
public class Survivor : Ability
{
    int power = 3;
    bool isEnabled = false;

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
        if (ev.Player != Owner || ABattle.Instance.IsLifeUsed[Owner] || Datas.BlockDamageTypes.Contains(ev.DamageHandler.Type))
            return;

        if (!isEnabled)
        {
            isEnabled = true;

            ev.IsAllowed = false;

            ev.Player.EnableEffect(EffectType.Blinded, 1, 3);
            ev.Player.EnableEffect(EffectType.Invisible, 1, 3);
            ev.Player.AddEffect(EffectType.MovementBoost, 20, 3);

            GodModePlayers.Add(ev.Player);

            Timing.CallDelayed(3f, () =>
            {
                if (GodModePlayers.Contains(ev.Player))
                    GodModePlayers.Remove(ev.Player);

                isEnabled = false;
            });

            ev.Player.AddHint("구사일생", $"<color={ABattle.RatingColor["영웅"]}>구사일생</color> 능력으로 인해 3초간 죽음을 피합니다. ({power - 1}번 남음)");

            ABattle.Instance.IsLifeUsed[Owner] = true;

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                ABattle.Instance.IsLifeUsed[Owner] = false;
            });

            if (power == 1)
                ev.Player.RemoveAbility(this);

            else
                power--;
        }
    }
}
