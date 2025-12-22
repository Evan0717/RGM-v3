using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using MEC;
using Mirror;
using MultiBroadcast;
using MultiBroadcast.API;
using PlayerRoles;
using Respawning;
using RGM.API.Features;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.LastOne)]
    class LastOne : Mode
    {
        public override string Name => "라스트 원";
        public override string Description => "혼자서 끝까지 살아남으세요.";
        public override string Detail =>
"""
랜덤한 맵에서, 최후까지 살아남아 승리를 쟁취하세요.

제한 시간은 3분이며, 3분이 지나면 버스터콜이 발생합니다.

<b>[Map Credit]</b>
@vasileii, @sleeplessbutter
""";
        public override string Color => "F8E0E6";
        public override string Map => Maps.GetRandomValue();

        public static LastOne Instance;

        List<ItemType> StartupItems = new List<ItemType>();

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.PauseWaves();

            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot += OnShot;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot -= OnShot;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            StartupItems = Items();

            foreach (var player in PlayerManager.List)
            {
                player.Role.Set(RoleTypeId.Tutorial);
                player.Position = Tools.GetObjectList("Spot Random").GetRandomValue().position;
                foreach (var item in StartupItems)
                    player.AddItem(item);
            }

            yield return Timing.WaitForSeconds(180f);

            Player BusterCall = Tools.GetRandomValue(PlayerManager.List.Where(x => x.IsAlive).ToList());

            foreach (var player in PlayerManager.List)
            {
                player.Position = BusterCall.Position;
                player.AddBroadcast(20, "<b><size=30>[<color=yellow>버스터콜</color>]</size></b>\n<size=20>모두가 한자리에 모입니다.</size>");
            }
        }

        public List<ItemType> Items()
        {
            List<ItemType> Guns = new List<ItemType>() { ItemType.GunA7, ItemType.GunE11SR, ItemType.GunShotgun, ItemType.GunCom45, ItemType.GunFSP9, ItemType.GunRevolver,
                ItemType.GunCOM18, ItemType.GunCrossvec, ItemType.GunLogicer, ItemType.GunFRMG0, ItemType.GunAK };
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

        public void OnDied(DiedEventArgs ev)
        {
            List<Player> pl = PlayerManager.List.Where(x => x.IsAlive).ToList();

            if (pl.Count() < 2)
            {
                Round.IsLocked = false;

                Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { pl[0] }, 5));
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
