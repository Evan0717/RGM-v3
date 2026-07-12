using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using RGM.API.Features;

namespace RGM.Modes.Abilities.Mythic;

[Ability("스피릿", "영혼 상태로 상시 전환됩니다! 5m 반경에 있는 플레이어가 SCP-1344 아이템을 가지고 있을 경우, 즉시 제거합니다.", AbilityCategory.Mythic, AbilityType.MYTHIC_SPIRIT)]
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

            foreach (var player in PlayerManager.List.Where(x => Vector3.Distance(x.Position, Owner.Position) <= 5))
            {
                try
                {
                    foreach (var item in player.Items)
                    {
                        if (item.Type == ItemType.SCP1344)
                            player.RemoveItem(item);
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"An error occurred while removing SCP-1344 from <b><i>{player.Nickname}</i></b> ({player.UserId}): {e}");
                }
            }

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