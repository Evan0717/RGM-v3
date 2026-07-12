using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
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

        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;

        _onStarted = Timing.RunCoroutine(OnStarted());
    }

    public override void OnDisabled()
    {
    }

    public void OnChangedItem(ChangedItemEventArgs ev)
    {
        if (ev.Item != null)
        {
            if (FlashLightSerial == ev.Player.CurrentItem.Serial && FlashLightSerial == ev.Item.Serial)
                ev.Player.AddHint("플래시라이트", $"손전등을 상대에게 비추면 <b><color={ABattle.RatingColor["전설"]}>플래시라이트</color></b> 능력을 사용할 수 있습니다.");
        }
    }

    public IEnumerator<float> OnStarted()
    {
        while (true)
        {
            foreach (var player in PlayerManager.List)
            {
                if (player.CurrentItem != null && FlashLightSerial == player.CurrentItem.Serial)
                {
                    if (Tools.TryGetLookPlayer(player, 45, out Player target, out RaycastHit? hit))
                    {
                        if (player != target && HitboxIdentity.IsEnemy(player.ReferenceHub, target.ReferenceHub))
                        {
                            Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, 0.8f);
                            target.EnableEffect(EffectType.Flashed, 1, 1f);
                        }
                    }
                }
            }

            yield return Timing.WaitForOneFrame;
        }
    }
}
