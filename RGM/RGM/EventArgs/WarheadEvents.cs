using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.EventArgs
{
    public static class WarheadEvents
    {
        public static IEnumerator<float> OnDetonating(Exiled.Events.EventArgs.Warhead.DetonatingEventArgs ev)
        {
            foreach (var _door in Door.List)
            {
                if (_door is BreakableDoor _bD)
                    _bD.IsDestroyed = true;
            }

            foreach (var player in PlayerManager.List.Where(x => x.IsAlive && x.Zone != ZoneType.Surface || 
            Physics.RaycastAll(x.Position, Vector3.down, 5, (LayerMask)1).Any(hit => hit.transform.parent != null && hit.transform.parent.name == "ElevatorChamber Gates(Clone)")
            ))
            {
                if (GodModePlayers.Contains(player))
                    GodModePlayers.Remove(player);

                player.Kill(UnityEngine.Random.Range(1, 6) == 1 ? "핵폭발이 당신을 죽음으로 가는 KTX에 태웠습니다." : "핵폭발로 인해 사망하였습니다.");
            }

            Timing.CallDelayed(2 * 60, () =>
            {
                GlobalPlayer.TryPlay("SCP - Breach", 1);
            });

            yield return Timing.WaitForSeconds(300);

            Warhead.IsLocked = true;
            Exiled.API.Features.Cassie.MessageTranslated("", $"시간이 너무 오래 걸립니다! 모두의 체력이 초당 5%씩 줄어듭니다!");

            PlayerManager.List.ToList().ForEach(x => x.EnableEffect(EffectType.PocketCorroding));

            while (true)
            {
                foreach (var player in PlayerManager.List)
                {
                    player.Health -= player.MaxHealth / 20;

                    if (player.Health <= 0 && player.IsAlive)
                        player.Kill("게임을 질질 끌어서 죽었습니다.");
                }

                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
