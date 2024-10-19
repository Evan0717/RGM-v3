using MultiBroadcast.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.EventArgs
{
    public static class Scp330Events
    {
        public static void OnInteractingScp330(Exiled.Events.EventArgs.Scp330.InteractingScp330EventArgs ev)
        {
            if (UnityEngine.Random.Range(1, 21) == 1)
            {
                ev.IsAllowed = false;
                ev.Player.TryAddCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Pink);

                ev.Player.AddBroadcast(10, $"<color=#FF00FF>핑크 캔디(5%, 정규)</color> 기믹이 적용되었습니다.");
            }
        }
    }
}
