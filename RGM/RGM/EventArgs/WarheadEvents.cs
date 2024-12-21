using Exiled.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

using static RGM.Variables.ServerManagers;
using UnityEngine;

namespace RGM.EventArgs
{
    public static class WarheadEvents
    {
        public static void OnStopping(Exiled.Events.EventArgs.Warhead.StoppingEventArgs ev)
        {
            if (AutoNuke)
                ev.IsAllowed = false;
        }

        public static void OnDetonating(Exiled.Events.EventArgs.Warhead.DetonatingEventArgs ev)
        {
            foreach (var player in Player.List.Where(x => x.IsAlive && x.Zone != ZoneType.Surface || 
            Physics.RaycastAll(x.Position, Vector3.down, 5, (LayerMask)1).Any(hit => hit.transform.parent != null && hit.transform.parent.name == "ElevatorChamber Gates(Clone)")
            ))
            {
                if (GodModePlayers.Contains(player))
                    GodModePlayers.Remove(player);

                player.Kill(UnityEngine.Random.Range(1, 6) == 1 ? "핵폭발이 당신을 죽음으로 가는 KTX에 태웠습니다." : "핵폭발로 인해 사망하였습니다.");
            }
        }
    }
}
