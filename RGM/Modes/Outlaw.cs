using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API;

namespace RGM.Modes
{
    public class Outlaw
    {
        public static Outlaw Instance;

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
            List<ItemType> FirearmList = new List<ItemType>() 
            { 
                ItemType.GunCOM15,
                ItemType.GunCOM18,
                ItemType.GunCom45,
                ItemType.GunFSP9,
                ItemType.GunE11SR,
                ItemType.GunFRMG0,
                ItemType.GunAK,
                ItemType.GunShotgun,
                ItemType.GunRevolver,
                ItemType.GunLogicer,
                ItemType.GunA7,
                ItemType.Jailbird,
                ItemType.ParticleDisruptor,
                ItemType.MicroHID
            };

            Item CurrentItem = player.AddItem(FirearmList[UnityEngine.Random.Range(0, FirearmList.Count())]);

            if (player.IsScp)
            {
                player.CurrentItem = CurrentItem;
            }
        }
    }
}
