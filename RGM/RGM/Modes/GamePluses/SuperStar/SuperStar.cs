using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Exiled.API.Features;
using HarmonyLib;
using MEC;
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
            Exiled.Events.Handlers.Player.Left += OnLeft;
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                Server.ExecuteCommand($"/speak {string.Join(".", Player.List.Select(x => x.Id))}. 1");

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            if (pl.Contains(ev.Player.UserId))
                pl.Remove(ev.Player.UserId);
        }
    }
}
