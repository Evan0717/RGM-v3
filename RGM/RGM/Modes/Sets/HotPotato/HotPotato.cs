using AFK;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using MEC;
using Mirror;
using MultiBroadcast.API;
using PlayerRoles;
using Respawning;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.Skeleton)]
    public class Skeleton : Mode
    {
        public override string Name => "폭탄 돌리기";
        public override string Description => "폭탄이 터지기 전에 다른 유저에게 넘기세요!";
        public override string Detail =>
"""
유저에 비례하여 <b>폭탄 플레이어의 수가 조정</b>됩니다. (10명 당 1마리 + 1마리)

어떤 수단을 사용하더라도 최후까지 살아남으세요!
""";
        public override string Color => "FA58D0";
        public override string Map => "hp";

        public static Skeleton Instance;

        List<Player> pl = new List<Player>();
        List<Player> BomberMans = new List<Player>();

        CoroutineHandle _onModeStarted;

        AudioClipPlayback audio;

        public override void OnEnabled()
        {
            Server.FriendlyFire = true;
            Round.IsLocked = true;
            Respawn.PauseWaves();
            AFKManager._kickTime = 120500;

            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Died += OnDied;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());

            audio = Tools.PlayGlobalAudio("Skeleton", 1, true);
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Died -= OnDied;

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
            }

            while (true)
            {
                pl.ShuffleList();

                for (float i = 1; i < pl.Count / 10 + 2; i++)
                {
                    try
                    {
                        Player BomberMan = Tools.GetRandomValue(pl.Where(x => pl.Contains(x) && !BomberMans.Contains(x)).ToList());

                        if (BomberMan != null && BomberMan.IsConnected)
                        {
                            BomberMan.Role.Set(RoleTypeId.Scp049, SpawnReason.ForceClass, RoleSpawnFlags.None);
                            BomberMans.Add(BomberMan);
                        }
                        else
                            pl.Remove(BomberMan);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                for (int i = 1; i < 11; i++)
                {
                    PlayerManager.List.ToList().ForEach(x => x.AddHint("폭탄돌리기 폭탄 알림", $"현재 폭탄 : {string.Join(" & ", BomberMans.Select(b => b.DisplayNickname))}\n{11 - i}초 후 폭탄이 터집니다.", 1.2f));

                    yield return Timing.WaitForSeconds(1f);
                }

                foreach (var bomber in BomberMans)
                {
                    try
                    {
                        if (pl.Contains(bomber))
                        {
                            pl.Remove(bomber);

                            var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Server.Host);
                            g.FuseTime = 0.1f;
                            g.MaxRadius = 0;
                            g.SpawnActive(bomber.Position, Server.Host);

                            bomber.Kill("폭탄을 넘기지 못해서 죽어버렸다네~");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                BomberMans.Clear();

                if (pl.Count() < 2)
                {
                    Round.IsLocked = false;

                    pl[0].Role.Set(RoleTypeId.ClassD, SpawnReason.ForceClass, RoleSpawnFlags.None);
                    PlayerManager.List.ToList().ForEach(x => x.AddBroadcast(20, $"승리자 : {pl[0].DisplayNickname}"));
                    Timing.RunCoroutine(Tools.SetWinner(new List<Player>() { pl[0] }, 5));
                    break;
                }

                PlayerManager.List.ToList().ForEach(x => x.AddHint("폭탄돌리기 펑", $"펑!", 2));

                yield return Timing.WaitForSeconds(2f);
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

            ev.Player.GetEffect(EffectType.MovementBoost).Intensity = 50;
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.Scp049 || ev.DamageHandler.Type == DamageType.CardiacArrest)
                ev.IsAllowed = false;

            if (BomberMans.Contains(ev.Attacker) && !BomberMans.Contains(ev.Player) && !ev.Player.IsNPC)
            {
                BomberMans.Remove(ev.Attacker);
                BomberMans.Add(ev.Player);

                Server.ExecuteCommand($"/fc {ev.Attacker.Id} ClassD 0");
                Server.ExecuteCommand($"/fc {ev.Player.Id} Scp049 0");
            }
        }

        public void OnDied(DiedEventArgs ev)
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
