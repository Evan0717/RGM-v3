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
using MultiBroadcast.API;
using RGM.API;
using Mirror;

namespace RGM.Modes
{
    public class TailCatcher
    {
        public static TailCatcher Instance;

        public List<Player> pl = new List<Player>();

        Player dj;

        public Player GetTarget(Player attacker)
        {
            int playerIndex = pl.IndexOf(attacker);
            int targetIndex = (playerIndex + 1) % pl.Count;

            return pl[targetIndex];
        }

        public void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Dying += OnDying;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load hp");

            Player.List.Where(x => !x.IsNPC).CopyTo(pl);

            dj = Tools.SpawnDJ("dj", RoleTypeId.Tutorial, new Vector3(82.51834f, 1014.692f, -50.10588f), "dj");

            GGUtils.Gtool.PlaySound("dj", "Initial_D_Dancing", VoiceChat.VoiceChatChannel.Intercom, 25, true);

            Timing.RunCoroutine(DJHeadBanging());

            foreach (var player in Player.List.Where(x => !x.IsNPC))
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.Position = new Vector3(83.91287f, 1014.692f, -37.13322f);
                player.AddItem(ItemType.GunCrossvec);
            }

            pl.ShuffleList();

            while (true)
            {
                foreach (var player in Player.List.Where(pl.Contains))
                {
                    Player target = GetTarget(player);

                    player.ShowHint($"당신의 타깃 : {target.DisplayNickname}", 1.2f);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> DJHeadBanging()
        {
            yield return Timing.WaitForSeconds(1f);

            bool HeadUp = true;

            while (true)
            {
                if (HeadUp)
                {
                    GGUtils.Gtool.Rotate(dj.ReferenceHub, new Vector3(0, -1f, 0));

                    HeadUp = false;

                    yield return Timing.WaitForSeconds(0.2f);
                }
                else
                {
                    GGUtils.Gtool.Rotate(dj.ReferenceHub, new Vector3(0, 1f, 0));

                    HeadUp = true;

                    yield return Timing.WaitForSeconds(0.15f);
                }
            }
        }

        public void OnSpawned(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            Server.ExecuteCommand($"/speak {ev.Player.Id} 1");
        }

        public void OnDroppingItem(Exiled.Events.EventArgs.Player.DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnDroppingAmmo(Exiled.Events.EventArgs.Player.DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnShot(Exiled.Events.EventArgs.Player.ShotEventArgs ev)
        {
            ev.Firearm.Ammo += 1;
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            Player target = GetTarget(ev.Attacker);

            if (ev.Player != target)
            {
                ev.Attacker.Hurt(ev.DamageHandler.Damage);

                ev.IsAllowed = false;
            }
        }


        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (pl.Contains(ev.Player))
            {
                pl.Remove(ev.Player);

                ev.IsAllowed = false;

                ev.Player.Role.Set(RoleTypeId.ClassD);
                ev.Player.Position = new Vector3(83.82303f, 1026.691f, -37.06291f);

                if (pl.Count < 2)
                {
                    Round.IsLocked = false;

                    Player.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {pl[0].Nickname}"));
                }
            }
        }
    }
}
