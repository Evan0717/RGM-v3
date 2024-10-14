using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Mirror;
using MultiBroadcast;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API;
using UnityEngine;

namespace RGM.Modes
{
    class Doppelganger
    {
        public static Doppelganger Instance;

        public List<string> pl = new List<string>();
        Player target = null;

        public void ApplyInfo(Player player)
        {
            try
            {
                player.Group.BadgeText = target.Group.BadgeText;
                player.Group.BadgeColor = target.Group.BadgeColor;
            }
            finally
            {
                player.DisplayNickname = target.DisplayNickname;
                player.CustomInfo = target.CustomInfo;
            }
        }

        public void OnEnabled()
        {
            Server.FriendlyFire = true;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            target = Tools.GetRandomValue(Player.List.Where(x => !x.IsScp).ToList());

            foreach (var player in Player.List.Where(x => x != target))
                ApplyInfo(target);
        }

        public void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            ApplyInfo(ev.Player);
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (ev.Player == target)
            {
                string AttackerName()
                {
                    if (ev.Attacker != null)
                        return ev.Attacker.DisplayNickname;

                    else if (ev.DamageHandler.Type == Exiled.API.Enums.DamageType.PocketDimension)
                        return Player.List.Where(x => x.Role == RoleTypeId.Scp106).ToList()[0].DisplayNickname;

                    else
                        return "알 수 없음";
                }

                foreach (var player in Player.List)
                {
                    player.ClearBroadcasts();
                    player.AddBroadcast(15, $"<size=25><b><color=red>표적</color>({target.DisplayNickname})이(가) 잡혔습니다!\n{AttackerName()}의 승리입니다!</b></size>");

                    if (player.IsAlive)
                        Server.ExecuteCommand($"/fc {player.Id} Tutorial 0");
                }
            }
        }
    }
}
