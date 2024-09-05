using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API;
using InventorySystem.Items.Usables.Scp330;

namespace RGM.Modes
{
    public class TrickorTreat
    {
        public static TrickorTreat Instance;

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

            yield break;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            List<CandyKindID> CandyList = Tools.EnumToList<CandyKindID>();

            for (int i=1; i<5; i++)
            {
                var toGive = CandyList[UnityEngine.Random.Range(0, CandyList.Count())];
                player.TryAddCandy(toGive);
            }
        }
    }
}
