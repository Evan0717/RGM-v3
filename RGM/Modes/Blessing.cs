using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;

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
                    int s = player.CurrentSpectatingPlayers.Count();
                    player.ShowHint($"현재 {s}명이 당신을 관전하고 있습니다.", 1.2f);

                    player.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 5 * s);
                    player.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 2 * s);
                    player.Heal(0.35f * s);
                    player.Scale = new Vector3(1 - s * 0.02f, 1 - s * 0.02f, 1 - s * 0.02f);
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            float s = ev.Player.CurrentSpectatingPlayers.Count();
            ev.DamageHandler.Damage = ev.DamageHandler.Damage * (0.3f * s);
        }
    }
}
