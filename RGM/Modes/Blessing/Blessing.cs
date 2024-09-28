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
using RGM.API;
using MultiBroadcast.API;

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

                        player.GetEffect(EffectType.MovementBoost).Intensity = (byte)(5 * s);
                        player.GetEffect(EffectType.DamageReduction).Intensity = (byte)(5 * s);
                        player.Heal(0.35f * s);

                        if (player.Role is Scp079Role scp079)
                            scp079.Energy += 0.35f * s;

                        if (s > 5)
                            player.IsUsingStamina = false;

                        else
                            player.IsUsingStamina = true;

                        if (s > 10)
                            player.IsBypassModeEnabled = true;

                        else
                            player.IsBypassModeEnabled = false;

                        if (s > 15)
                            player.EnableEffect(EffectType.Ghostly, 1, 1.2f);

                        if (s > 20)
                            player.EnableEffect(EffectType.Invisible, 1, 1.2f);

                        if (s > 25)
                        {
                            if (UnityEngine.Random.Range(1, 51) == 1)
                            {
                                Item Item = player.AddItem(RGM.GetRandomValue(Tools.EnumToList<ItemType>()));

                                if (player.IsScp)
                                    player.CurrentItem = Item;
                            }
                        }

                        if (s > 30)
                        {
                            player.IsNoclipPermitted = true;

                            player.AddBroadcast(1, "<b><i>[ALT] 키를 눌러 <color=red>신의 권능</color>을 사용할 수 있습니다!!!</i></b>");
                        }

                        else
                            player.IsNoclipPermitted = false;
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
