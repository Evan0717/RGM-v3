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
using PlayerRoles;

namespace RGM.Modes
{
    public class Distancing
    {
        public static Distancing Instance;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                List<Player> DamagePlayers = new List<Player>();

                foreach (var p1 in Player.List)
                {
                    foreach (var p2 in Player.List)
                    {
                        if (p1 != p2 && Vector3.Distance(p1.Position, p2.Position) < 7.5f)
                        {
                            if (!DamagePlayers.Contains(p1))
                                DamagePlayers.Add(p1);

                            if (!DamagePlayers.Contains(p2))
                                DamagePlayers.Add(p2);
                        }
                    }
                }

                foreach (var player in DamagePlayers.Where(x => x.Role.Type != RoleTypeId.Scp079))
                {
                    player.Health -= player.MaxHealth / 50;

                    if (player.IsAlive)
                    {
                        player.EnableEffect(EffectType.Poisoned, 1, 1.5f);

                        if (player.Health <= 0)
                            player.Kill("사회가 당신과 거리를 두었습니다.");
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}