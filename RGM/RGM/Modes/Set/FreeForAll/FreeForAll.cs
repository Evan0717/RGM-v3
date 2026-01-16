using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using MEC;
using Mirror;
using MultiBroadcast;

using Respawning;
using RGM.API.Features;
using UnityEngine;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.FreeForAll)]
    class FreeForAll : Mode
    {
        public override string Name => "개인전";
        public override string Description => "최후의 1인이 되세요!";
        public override string Detail =>
"""
랜덤한 문으로 순간이동한 후 랜덤하게 지급되는 아이템으로 싸움을 시작합니다.

<i>모든 문은 잠겨 있습니다.</i>
""";
        public override string Color => "FA58F4";

        public static FreeForAll Instance;

        List<Player> pl = new List<Player>();
        List<ItemType> StartupItems = new List<ItemType>();
        Door door;

        CoroutineHandle _onModeStarted;

        List<ItemType> Items()
        {
            List<ItemType> Guns = new List<ItemType>() { ItemType.GunA7, ItemType.GunE11SR, ItemType.GunShotgun, ItemType.GunCom45, ItemType.GunFSP9, ItemType.GunRevolver,
                ItemType.GunCOM18, ItemType.GunCrossvec, ItemType.GunLogicer, ItemType.GunFRMG0, ItemType.GunAK, ItemType.Jailbird, ItemType.ParticleDisruptor };
            List<ItemType> Ammos = new List<ItemType>() { ItemType.Ammo12gauge, ItemType.Ammo44cal, ItemType.Ammo556x45, ItemType.Ammo762x39, ItemType.Ammo9x19 };
            List<ItemType> CDItems = new List<ItemType>() { ItemType.Medkit, ItemType.Painkillers, ItemType.Radio, ItemType.GrenadeFlash };
            List<ItemType> Items = new List<ItemType>();

            Items.Add(Tools.GetRandomValue(Guns));

            foreach (var item in CDItems)
            {
                if (UnityEngine.Random.Range(1, 3) == 1)
                    Items.Add(item);
            }

            return Items;
        }

        public override void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.PauseWaves();
            Door.List.ToList().ForEach(x => x.Lock(1205, Exiled.API.Enums.DoorLockType.Lockdown079));

            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot += OnShot;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Door.List.ToList().ForEach(x => x.Unlock());

            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot -= OnShot;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            door = Tools.GetRandomValue(Door.List.ToList());
            StartupItems = Items();

            PlayerManager.List.ToList().CopyTo(pl);
            PlayerManager.List.ToList().ForEach(Spawned);

            yield return Timing.WaitForSeconds(180f);

            Player BusterCall = Tools.GetRandomValue(PlayerManager.List.Where(x => x.IsAlive).ToList());

            foreach (var player in PlayerManager.List)
            {
                player.Position = BusterCall.Position;
                player.AddBroadcast(5, "<b><size=30>[<color=yellow>버스터콜</color>]</size></b>\n<size=20>모두가 한자리에 모입니다.</size>");
            }
        }

        public void OnDying(DyingEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                if (pl.Count() < 2)
                {
                    Round.IsLocked = false;

                    PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {pl[0].DisplayNickname}"));
                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { pl[0] }, 5));
                }
            }
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Spawned(ev.Player);
        }

        public void Spawned(Player player)
        {
            PlayerManager.List.ToList().ForEach(x => x.DisableEffect(Exiled.API.Enums.EffectType.FogControl));
            Timing.CallDelayed(0.1f, () => PlayerManager.List.ToList().ForEach(x => x.EnableEffect(Exiled.API.Enums.EffectType.FogControl)));

            if (player.Role.Type != PlayerRoles.RoleTypeId.NtfSpecialist && pl.Contains(player))
            {
                player.Role.Set(PlayerRoles.RoleTypeId.NtfSpecialist);
                player.Position = new Vector3(door.Position.x, door.Position.y + 2, door.Position.z);

                player.ClearInventory();

                foreach (var item in StartupItems)
                    player.AddItem(item);
            }
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnShot(ShotEventArgs ev)
        {
            ev.Player.AddAmmo(ev.Firearm.AmmoType, 1);
        }
    }
}
