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
using RGM.API.Features;
using Mirror;
using Respawning;

using static RGM.Variables.ServerManagers;

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

        public static TailCatcter Instance;

        public List<Player> pl = new List<Player>();

        Player dj;

        public Player GetTarget(Player attacker)
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
            Tools.LoadMap($"hp");

            Tools.PlayGlobalAudio("Initial_D_Dancing", 1, true);

            Player.List.Where(x => !x.IsNPC).CopyTo(pl);

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
            IntercomPlayers.Add(ev.Player);
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
            ev.Firearm.MagazineAmmo += 1;
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

                if (pl.Count() < 2)
                {
                    Round.IsLocked = false;

                    Player.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {pl[0].DisplayNickname}"));
                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { pl[0] }, 5));
                }
            }
        }
    }
}
