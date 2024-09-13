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
using InventorySystem.Items.Usables.Scp244.Hypothermia;

namespace RGM.Modes
{
    public class IceAge
    {
        public static IceAge Instance;

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
                        if (p1 != p2 && Vector3.Distance(p1.Position, p2.Position) < 7.5f)
                        {
                            if (!PassPlayers.Contains(p1))
                            {
                                PassPlayers.Add(p1);
                                PassPlayers.Add(p2);
                            }
                        }
                    }
                }

                foreach (var player in Player.List.Where(x => !PassPlayers.Contains(x)))
                {
                    player.EnableEffect(Exiled.API.Enums.EffectType.Hypothermia, 40, 1.5f);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
