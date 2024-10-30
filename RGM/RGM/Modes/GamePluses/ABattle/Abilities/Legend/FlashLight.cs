using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes.Abilities.Legend;

[Ability("플래시라이트", "지급된 손전등을 들고 상대를 쳐다보면 눈뽕 공격을 가할 수 있습니다.", AbilityCategory.Legend, AbilityType.LEGEND_FLASHLIGHT)]
public class FlashLight : Ability
{
    CoroutineHandle _onStarted;
    ushort FlashLightSerial = 0;

    public override void OnEnabled()
    {
        Item fl = Owner.AddItem(ItemType.Flashlight);
        FlashLightSerial = fl.Serial;

        if (Owner.IsScp)
            Owner.CurrentItem = fl;

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;

        _onStarted = Timing.RunCoroutine(OnStarted());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;

        Timing.KillCoroutines(_onStarted);
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Player != Owner)
            return;

        if (ev.Item != null)
        {
            if (FlashLightSerial == ev.Item.Serial)
                ev.Player.ShowHint($"손전등을 상대에게 비추면 <b><color={ABattle.RatingColor["전설"]}>플래시라이트</color></b> 능력을 사용할 수 있습니다.");
        }
    }

    public IEnumerator<float> OnStarted()
    {
        while (true)
        {
            if (Owner.CurrentItem != null && FlashLightSerial == Owner.CurrentItem.Serial)
            {
                if (Tools.TryGetLookPlayer(Owner, 45, out Player target))
                {
                    if (Owner != target && Owner.LeadingTeam != target.LeadingTeam)
                    {
                        Hitmarker.SendHitmarkerDirectly(Owner.ReferenceHub, 0.8f);
                        target.EnableEffect(EffectType.Flashed, 1, 1f);
                    }
                }
            }

            yield return Timing.WaitForSeconds(0.1f);
        }
    }
}
