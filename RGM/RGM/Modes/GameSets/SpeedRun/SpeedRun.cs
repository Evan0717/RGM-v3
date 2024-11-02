using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MapGeneration.Distributors;
using MEC;
using Mirror;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    class SpeedRun
    {
        public static SpeedRun Instance;

        public List<Player> pl = new List<Player>();
        List<ItemType> Standards = new List<ItemType>()
        {
            ItemType.Flashlight
        };
        List<ItemType> Extensions = new List<ItemType>()
        {
            ItemType.KeycardO5,
            ItemType.Adrenaline,
            ItemType.Coin,
            ItemType.Lantern,
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.SCP330
        };
        List<ItemType> ItemsList = new List<ItemType>();

        public bool IsEnd = false;

        public void OnEnabled()
        {
            Round.IsLocked = true;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
            Exiled.Events.Handlers.Warhead.Stopping += OnStopping;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Player.List.ToList().CopyTo(pl);

            Warhead.Start();

            foreach (var Item in Standards)
                ItemsList.Add(Item);

            foreach (var Item in Extensions)
            {
                if (UnityEngine.Random.Range(1, 3) == 1)
                    ItemsList.Add(Item);
            }

            Tools.TryInstallMode("FriendlyFire");

            foreach (var player in Player.List)
                Spawned(player);

            yield break;
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            if (pl.Contains(player))
            {
                if (player.Role.Type == RoleTypeId.ChaosConscript)
                    return;

                if (player.Role.Type != RoleTypeId.ClassD)
                    player.Role.Set(RoleTypeId.ClassD);

                foreach (var Item in ItemsList)
                    player.AddItem(Item);
            }
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count < 2)
                    Round.IsLocked = false;
            }
        }

        public void OnEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs ev)
        {
            if (ev.Player.Role.Type == RoleTypeId.ClassD)
            {
                Round.IsLocked = false;

                if (!IsEnd)
                {
                    Player.List.ToList().ForEach(x => x.AddBroadcast(20, $"<b><size=30><최초 탈출자 : {ev.Player.DisplayNickname}></size></b>"));
                    IsEnd = true;
                }
            }
        }

        public void OnStopping(Exiled.Events.EventArgs.Warhead.StoppingEventArgs ev)
        {
            Warhead.Detonate();
            Player.List.ToList().ForEach(x => x.AddBroadcast(10, $"<size=30>{ev.Player.DisplayNickname}(이)가 <color=red>핵</color>을 강제로 폭파시켰습니다!</size>"));
        }
    }
}
