using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Exiled.API.Features;
using HarmonyLib;
using PlayerRoles;
using UnityEngine;
using VoiceChat;
using VoiceChat.Playbacks;
using static RGM.Modes.FriendlyFire;

namespace RGM.Modes
{
    class SuperStar
    {
        public static SuperStar Instance;

        public List<string> pl = new List<string>();

        public void OnEnabled()
        {
            Task.WhenAll(
                 OnModeStarted()
                 );

            Exiled.Events.Handlers.Player.Left += OnLeft;

            Harmony harmony = new Harmony($"SuperStar - {DateTime.Now.Ticks}");
            harmony.Patch(AccessTools.Method(typeof(GlobalChatIndicator), nameof(GlobalChatIndicator.TryGetIcon), [typeof(GlobalChatIconType), typeof(ReferenceHub), typeof(Texture)]),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(TryGetIconPostfix), nameof(TryGetIconPostfix.Postfix))));
        }

        public async Task OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List)
                    if (!pl.Contains(player.UserId))
                    {
                        Server.ExecuteCommand($"/speak {player} enable");
                        pl.Add(player.UserId);
                    }

                await Task.Delay(1000);
            }
        }

        public void OnLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            if (pl.Contains(ev.Player.UserId))
                pl.Remove(ev.Player.UserId);
        }

        public class TryGetIconPostfix
        {
            public static void Postfix(ref bool __result, GlobalChatIconType icon, ReferenceHub owner, out Texture result)
            {
                result = null;

                if (icon == GlobalChatIconType.None)
                    __result = false;

                else
                {
                    if (!(owner == null))
                    {
                        IAvatarRole avatarRole = owner.roleManager.CurrentRole as IAvatarRole;
                        if (avatarRole != null)
                        {
                            result = avatarRole.RoleAvatar;
                            __result = true;
                        }
                        else
                            __result = false;

                    }
                    else
                        __result = false;
                }
            }
        }
    }
}
