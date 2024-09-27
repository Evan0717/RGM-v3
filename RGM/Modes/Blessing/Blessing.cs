using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;

namespace RGM.Modes
{
    public class Blessing
    {
        public static Blessing Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (player.IsAlive)
                    {
                        int s = player.CurrentSpectatingPlayers.Count();
                        player.ShowHint($"현재 {s}명이 당신을 관전하고 있습니다.", 1.2f);

                        player.EnableEffect(EffectType.MovementBoost, 5 * s);
                        player.EnableEffect(EffectType.DamageReduction, 2 * s);
                        player.Heal(0.35f * s);
                        player.Scale = new Vector3(1 - s * 0.02f, 1 - s * 0.02f, 1 - s * 0.02f);
                    }
                    else
                    {
                        if (player.Role is SpectatorRole spectator)
                        {
                            if (spectator.SpectatedPlayer != null)
                            {
                                int s = spectator.SpectatedPlayer.CurrentSpectatingPlayers.Count();
                                player.ShowHint($"현재 {s}명이 이 플레이어를 관전하고 있습니다.", 1.2f);
                            }
                        }
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                float s = ev.Attacker.CurrentSpectatingPlayers.Count();
                ev.DamageHandler.Damage = ev.DamageHandler.Damage + ev.DamageHandler.Damage * (0.3f * s);
            }
        }
    }
}
