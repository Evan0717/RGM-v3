using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API;
using UnityEngine;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using PlayerRoles;

namespace RGM.Modes
{
    public class Siberia
    {
        public static Siberia Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            foreach (var player in Player.List)
            {
                Spawned(player);
            }

            while (true)
            {
                List<Player> Players = Player.List.ToList();
                List<Player> PassPlayers = new List<Player>();

                foreach (var p1 in Players)
                {
                    foreach (var p2 in Player.List.Where(x => !PassPlayers.Contains(x)))
                    {
                        if (p1 != p2 && Vector3.Distance(p1.Position, p2.Position) <= 3f)
                        {
                            if (!PassPlayers.Contains(p1))
                                PassPlayers.Add(p1);

                            if (!PassPlayers.Contains(p2))
                                PassPlayers.Add(p2);
                        }
                    }
                }

                foreach (var player in Player.List.Where(x => !PassPlayers.Contains(x) && x.IsAlive))
                    player.EnableEffect(EffectType.Hypothermia, 255, 1.5f);

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public async void Spawned(Player player)
        {
            await Task.Delay(1000);
            player.EnableEffect(EffectType.FogControl, 7);

            if (player.Role.Type == RoleTypeId.Scp079)
                player.Health += 100000;
        }
    }
}
