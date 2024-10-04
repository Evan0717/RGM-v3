using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using MEC;
using Mirror;
using UnityEngine;
using PlayerRoles;
using HarmonyLib;
using PlayerRoles.Visibility;
using GameCore;
using RGM.Commands;
using Utils;

namespace RGM.Modes
{
    class Spirit
    {
        public static Spirit Instance;

        List<Player> spirits = new List<Player>();

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Player.Hurt += OnHurt;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (spirits.Contains(player))
                    {
                        player.EnableEffect(EffectType.Invisible);
                        player.EnableEffect(EffectType.Ghostly);
                    }
                }

                yield return Timing.WaitForSeconds(1.5f);
            }
        }

        public async void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (spirits.Contains(ev.Player) || ev.DamageHandler.Type == DamageType.Falldown)
            {
                ev.Player.ShowHint($"성불했습니다..", 3);
                spirits.Remove(ev.Player);
            }
            else
            {
                for (int i = 1; i < 6; i++)
                {
                    ev.Player.ShowHint($"{6 - i}초 뒤 영혼 상태에 돌입합니다.", 1.2f);
                    await Task.Delay(1000);
                }
                spirits.Add(ev.Player);

                Server.ExecuteCommand($"/fc {ev.Player.Id} Tutorial 1");
            }
        }

        public void OnShot(Exiled.Events.EventArgs.Player.ShotEventArgs ev)
        {
            if (spirits.Contains(ev.Player))
                ev.Player.DisableEffect(EffectType.Invisible);
        }

        public void OnHurt(Exiled.Events.EventArgs.Player.HurtEventArgs ev)
        {
            if (ev.Attacker != null && spirits.Contains(ev.Attacker))
                ev.Attacker.DisableEffect(EffectType.Invisible);

            if (spirits.Contains(ev.Player))
                ev.Player.DisableEffect(EffectType.Invisible);
        }
    }
}
