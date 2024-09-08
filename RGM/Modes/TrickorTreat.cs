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
            Exiled.Events.Handlers.Player.Dying += OnDying;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(0.5f);

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
            var p = player.AddItem(ItemType.SCP330);
            if (p is Scp330 candy)
            {
                List<CandyKindID> Candies = new List<CandyKindID>
                    {
                        (CandyKindID)UnityEngine.Random.Range(1, 8),
                        (CandyKindID)UnityEngine.Random.Range(1, 8),
                        (CandyKindID)UnityEngine.Random.Range(1, 8)
                    };

                foreach (var Candy in Candies)
                {
                    candy.AddCandy(Candy);
                }
            }
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            List<CandyKindID> CandyList = Tools.EnumToList<CandyKindID>();
            {
                var toGive = CandyList[UnityEngine.Random.Range(0, CandyList.Count())];
                ev.Attacker.TryAddCandy(toGive);
            }
        }
    }
};
