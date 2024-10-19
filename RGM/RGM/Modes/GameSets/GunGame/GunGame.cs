using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using HarmonyLib;
using MEC;
using Mirror;
using MultiBroadcast;
using UnityEngine;
using Exiled.API.Enums;
using PlayerRoles;
using MultiBroadcast.API;
using RGM.API.Features;

namespace RGM.Modes
{
    class GunGame
    {
        public static GunGame Instance;

        public Dictionary<Player, int> Stage = new Dictionary<Player, int>(); 
        public bool IsEnd = false;

        public List<ItemType> GunsList = new List<ItemType>()
        { 
            ItemType.Jailbird,
            ItemType.ParticleDisruptor,
            ItemType.GunFRMG0,
            ItemType.GunLogicer,
            ItemType.GunE11SR,
            ItemType.GunAK,
            ItemType.GunCrossvec,
            ItemType.GunA7,
            ItemType.GunFSP9,
            ItemType.GunCom45,
            ItemType.GunRevolver,
            ItemType.GunCOM18,
            ItemType.GunCOM15,
            ItemType.MicroHID,
            ItemType.Lantern
        };

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Dying += OnDying;

            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(ScoreBoard());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;
            foreach (var Door in Door.List.Where(x => x.Zone == ZoneType.HeavyContainment))
            {
                if (Door.IsCheckpoint || Door.IsElevator)
                    Door.Lock(1205, DoorLockType.Lockdown079);

                else
                    Door.IsOpen = true;
            }

            Server.ExecuteCommand("/atkill all");

            while (!IsEnd)
            {
                foreach (var player in Player.List)
                {
                    if (Stage.ContainsKey(player))
                    {
                        if (Stage[player] >= GunsList.Count - 1)
                            IsEnd = true;

                        if (player.IsDead)
                            PlayerSpawn(player);
                    }
                    else
                        Stage.Add(player, 0);
                }

                yield return Timing.WaitForSeconds(1f);
            }

            Player topPlayer = Stage.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            Round.IsLocked = false;

            Player.List.ToList().ForEach(x => x.AddBroadcast(20, $"<b><color=yellow>{topPlayer.Nickname}</color></b>(이)가 <color=#088A08>Gun Game</color>에서 우승했습니다!"));
        }

        public IEnumerator<float> ScoreBoard()
        {
            IEnumerator<float> Processing()
            {
                Map.CleanAllItems();
                Map.CleanAllRagdolls();

                foreach (var player in Player.List)
                {
                    if (Stage.ContainsKey(player))
                    {
                        player.ClearPlayerBroadcasts();
                        Player topPlayer = Stage.OrderByDescending(x => x.Value).FirstOrDefault().Key;
                        player.AddBroadcast(2, $"<size=25><b><color=yellow>선두주자(1st) 플레이어</color> - {topPlayer.Nickname}({Stage[topPlayer]}점)</b></size>\n" +
                            $"<size=20><i>우승까지 <color=red>{GunsList.Count - Stage[player] - 1}</color>킬({Stage[player]}점) 남았습니다.</i></size>");
                    }
                }

                yield return 0f;
            }

            while (!IsEnd)
            {
                Timing.RunCoroutine(Processing());

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void PlayerSpawn(Player player)
        {
            Door SelectedDoor = Tools.GetRandomValue(Door.List.Where(x => !x.IsElevator && !x.IsPartOfCheckpoint && x.Zone == ZoneType.HeavyContainment && 
            !new List<RoomType>(){ RoomType.Hcz939, RoomType.Hcz079, RoomType.Hcz049, RoomType.Hcz106, RoomType.HczNuke }.Contains(x.Room.Type)).ToList());

            if (player.Role.Type != RoleTypeId.ClassD)
                player.Role.Set(RoleTypeId.ClassD);
            player.Health = player.MaxHealth;
            player.EnableEffect(EffectType.Flashed, 1, 0.1f);
            player.ClearInventory();
            player.AddItem(GunsList[Stage[player]]);
            player.Position = new Vector3(SelectedDoor.Position.x, SelectedDoor.Position.y + 2, SelectedDoor.Position.z);

            List<string> AmmosList = new List<string>() { "19", "22", "27", "28", "29" };

            Server.ExecuteCommand($"/give {player.Id} {string.Join(".", AmmosList)}.");
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (Stage.ContainsKey(ev.Player))
            {

                Stage[ev.Attacker]++;
                ev.Attacker.ClearInventory();
                ev.Attacker.AddItem(GunsList[Stage[ev.Attacker]]);
                PlayerSpawn(ev.Player);
                ev.IsAllowed = false;
            }
        }
    }
}
