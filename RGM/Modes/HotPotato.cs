using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Interactables.Interobjects.DoorUtils;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using Exiled.API.Enums;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Extensions;

namespace RGM.Modes
{
    public class HotPotato
    {
        public static HotPotato Instance;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 1000;

            yield return Timing.WaitForSeconds(1f);

            foreach (var p in Player.List)
            {
                p.Role.Set(RoleTypeId.ClassD);
                Timing.CallDelayed(0.25f, () =>
                {
                    p.Position = RoleTypeId.ChaosRifleman.GetRandomSpawnLocation().Position;
                });
            }

            yield return Timing.WaitForSeconds(5f);

            Player firstBomb = Player.List.Where(x => x.Role == RoleTypeId.ClassD).ToList().RandomItem();
            firstBomb.Role.Set(RoleTypeId.Scp049, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);

            /*
            var camera = firstBomb.Camera;
            firstBomb.Camera.SetPositionAndRotation(camera.position, camera.rotation);
            */

            if (Player.List.Where(x => x.IsAlive).Count() >= 10)
            {
                firstBomb = Player.List.Where(x => x.Role == RoleTypeId.ClassD).ToList().RandomItem();
                firstBomb.Role.Set(RoleTypeId.Scp049, SpawnReason.ForceClass, RoleSpawnFlags.None);
            }
            Timing.RunCoroutine(BoomSequence());
            Timing.RunCoroutine(HighKiller());
            yield break;
        }

        public IEnumerator<float> BoomSequence()
        {
            for (; ; )
            {
                int targetTime = Player.List.Where(x => x.IsAlive).Count() >= 10 ? 10 : 15;
                for (int t = 0; t < targetTime; t++)
                {
                    var l1 = Player.List.Where(x => x.IsScp);
                    var bombs = new List<string>();
                    foreach (var l in l1)
                        bombs.Add(l.Nickname);

                    foreach (var all in Player.List)
                    {
                        all.ShowHint($"<color=#F6CECE>폭탄의 폭발까지</color>: {targetTime - t - 1}초" +
                            $"\n<color=#F3E2A9>폭탄 보유자</color>: {string.Join(" <b><color=#BDBDBD>&</color></b> ", bombs)}", 2);
                    }
                    yield return Timing.WaitForSeconds(1f);
                }
                foreach (var boom in Player.List.Where(x => x.IsScp))
                {
                    boom.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);
                    var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, boom);
                    g.FuseTime = 0f;
                    g.SpawnActive(boom.Position, boom);
                    boom.Health = 1;
                }
                foreach (var all in Player.List)
                {
                    all.ShowHint($"<color=#F79F81>쾅!</color>", 5);
                }
                yield return Timing.WaitForSeconds(2f);
                List<Player> newList = Player.List.Where(x => x.Role == RoleTypeId.ClassD).ToList();
                if (newList.Count <= 1)
                {
                    Round.IsLocked = false;
                    yield break;
                }
                for (int i = newList.Count >= 10 ? 0 : 1; i < 2; i++)
                {
                    Player newBomb = newList.RandomItem();
                    newList.Remove(newBomb);
                    newBomb.Role.Set(RoleTypeId.Scp049, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);
                    /*
                    var rotation = newBomb.Rotation;
                    Timing.CallDelayed(0.1f, () =>
                    {
                        newBomb.Rotation = rotation;
                    });
                    */
                }
            }
        }

        public IEnumerator<float> HighKiller()
        {
            for (; ; )
            {
                yield return Timing.WaitForSeconds(2.5f);
            }
        }

        public void OnReceivingEffect(Exiled.Events.EventArgs.Player.ReceivingEffectEventArgs ev)
        {
            if (ev.Intensity <= 0) return;
            if (!(ev.Effect is CardiacArrest ca)) return;
            Player attacker = Player.Get(ca._attacker.Hub);
            ev.Player.Role.Set(RoleTypeId.Scp049, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);

            attacker.Role.Set(RoleTypeId.ClassD, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);
            ev.IsAllowed = false;
        }

        public void OnRespawningTeam(Exiled.Events.EventArgs.Server.RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);
        }

        public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
