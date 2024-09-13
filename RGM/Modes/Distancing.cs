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
                List<Player> PassPlayers = new List<Player>();

                foreach (var p1 in Player.List)
                {
                    foreach (var p2 in Player.List)
                    {
                        if (p1 != p2 && Vector3.Distance(p1.Position, p2.Position) > 10f)
                        {
                            if (!PassPlayers.Contains(p1))
                                PassPlayers.Add(p1);

                            if (!PassPlayers.Contains(p2))
                                PassPlayers.Add(p2);
                        }
                    }
                }

                foreach (var player in Player.List.Where(x => !PassPlayers.Contains(x)))
                    player.Health -= player.MaxHealth / 10;

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}