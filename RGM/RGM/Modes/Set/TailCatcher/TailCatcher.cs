using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;
using PlayerRoles;

using RGM.API.Features;
using Mirror;
using Respawning;

using static RGM.Variables.Variable;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.TailCatcter)]
    public class TailCatcter : Mode
    {
        public override string Name => "꼬리 잡기";
        public override string Description => "절대로 타깃이 아닌 유저를 쏘지 마세요! 꼬리가 잡히지 않도록 하십시오.";
        public override string Detail =>
"""
<color=red>꼬리가 아닌 유저를 공격하면 데미지가 반사됩니다.</color>

눈치 게임과 비슷하군요!
""";
        public override string Color => "A9F5BC";
        public override string Map => "hp";

        public static TailCatcter Instance;

        List<Player> pl = new List<Player>();

        CoroutineHandle _onModeStarted;

        AudioClipPlayback audio;

        Player GetTarget(Player attacker)
        {
            int playerIndex = pl.IndexOf(attacker);
            int targetIndex = (playerIndex + 1) % pl.Count;

            return pl[targetIndex];
        }

        public override void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.PauseWaves();

            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Dying += OnDying;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());

            audio = Tools.PlayGlobalAudio("Initial_D_Dancing", 1, true);
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Dying += OnDying;

            Timing.KillCoroutines(_onModeStarted);

            audio.IsPaused = true;
        }

        public IEnumerator<float> OnModeStarted()
        {
            PlayerManager.List.Where(x => !x.IsNPC).CopyTo(pl);

            foreach (var door in Door.List)
            {
                door.Lock(DoorLockType.Warhead);
            }

            foreach (var player in PlayerManager.List.Where(x => !x.IsNPC))
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.Position = GameObject.Find("[SP] Base").transform.position;
                player.AddItem(ItemType.GunCrossvec);
            }

            pl.ShuffleList();

            while (true)
            {
                foreach (var player in PlayerManager.List.Where(pl.Contains))
                {
                    Player target = GetTarget(player);

                    player.AddHint("꼬리 잡기 타겟 알림", $"당신의 타깃 : {target.DisplayNickname}", 1.2f);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            Server.ExecuteCommand($"/speak {ev.Player.Id} 1");
            IntercomPlayers.Add(ev.Player);
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
            ev.Firearm.MagazineAmmo += 1;
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            Player target = GetTarget(ev.Attacker);

            if (ev.Player != target)
            {
                ev.Attacker.Hurt(ev.DamageHandler.Damage);

                ev.IsAllowed = false;
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
    }
}
