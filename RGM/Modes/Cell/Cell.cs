using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Mirror;
using MultiBroadcast;
using RGM.API;
using UnityEngine;

namespace RGM.Modes
{
    class Cell
    {
        public static Cell Instance;

        public List<Player> pl = new List<Player>();

        public void OnEnabled()
        {
            Round.IsLocked = true;
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand("/mp load cell");
            Player BadLucky = Tools.GetRandomValue(Player.List.ToList());
            Player.List.ToList().CopyTo(pl);

            yield return Timing.WaitForSeconds(1f);

            foreach (var player in Player.List)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
                player.Position = new Vector3(118.7332f, 1000.379f, -41.59417f);

                if (player == BadLucky)
                    Server.ExecuteCommand($"/drop {player.Id} 31 1");
            }
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count < 2)
                    Round.IsLocked = false;
            }
        }
    }
}
