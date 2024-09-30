using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API;
using PlayerRoles;
using Exiled.API.Enums;

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
            yield return Timing.WaitForSeconds(1f);

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
            if (player.Role.Type != RoleTypeId.Scp079)
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

                Item CurrentItem = player.AddItem(Tools.GetRandomValue(FirearmList));

                if (CurrentItem is Firearm firearm)
                {
                    if (firearm.AmmoType != AmmoType.None)
                    {
                        for (int i = 0; i < 3; i++)
                            player.AddAmmo(firearm.AmmoType, firearm.MaxAmmo);
                    }
                }

                if (player.IsScp)
                    player.CurrentItem = CurrentItem;
            }
        }
    }
}
