using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem.Items.Usables.Scp244;
using MEC;
using Mirror;
using MultiBroadcast.API;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    class BombParty
    {
        public static BombParty Instance;

        public List<Player> pl = new List<Player>();

        public void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10f);

            Server.ExecuteCommand($"/mp load bp");

            Player.List.ToList().CopyTo(pl);

            foreach (var player in Player.List)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.ClassD);
                player.Position = new Vector3(-0.09375f, 1000.957f, -7.28125f);
            }

            int t = 0;

            while (true)
            {
                t += 3;

                var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Server.Host);
                g.FuseTime = 3f;
                g.SpawnActive(GetRandomPosition(), Server.Host);

                if (t > 30)
                {
                    if (UnityEngine.Random.Range(1, 4) == 1)
                    {
                        var g1 = (FlashGrenade)Item.Create(ItemType.GrenadeFlash);
                        g1.FuseTime = 2f;
                        g1.SpawnActive(GetRandomPosition());
                    }
                }
                if (t > 60)
                {
                    if (UnityEngine.Random.Range(1, 4) == 1)
                    {
                        var g1 = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Server.Host);
                        g1.FuseTime = 5f;
                        g1.SpawnActive(GetRandomPosition(), Server.Host);
                    }
                }
                if (t > 90)
                {
                    if (UnityEngine.Random.Range(1, 4) == 1)
                    {
                        var scp244 = (Scp244)Item.Create(Tools.GetRandomValue(new List<ItemType>() { ItemType.SCP244a, ItemType.SCP244b }), Server.Host);
                        scp244.CreatePickup(GetRandomPosition(), new Quaternion(45, 0, 0, 0));
                    }
                }
                if (t > 120)
                {
                    if (UnityEngine.Random.Range(1, 4) == 1)
                    {
                        var scp018 = (Scp018)Item.Create(ItemType.SCP018, Server.Host);
                        scp018.SpawnActive(GetRandomPosition(), Server.Host);
                    }
                }

                Player.List.ToList().ForEach(x => x.DisableAllEffects());
                yield return Timing.WaitForSeconds(3f);
            }
        }

        public Vector3 GetRandomPosition()
        {
            return new Vector3(UnityEngine.Random.Range(-9.941405f, 10.92998f), 1004.188f, UnityEngine.Random.Range(-15.76172f, 2.550781f));
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count < 2)
                {
                    Round.IsLocked = false;

                    Player.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {pl[0].Nickname}"));
                }
            }
        }
    }
}
