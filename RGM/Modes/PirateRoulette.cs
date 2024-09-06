using AdminToys;
using CommandSystem.Commands.RemoteAdmin;
using CustomPlayerEffects;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Interactables.Interobjects.DoorUtils;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RGM.Modes
{
    public class PirateRoulette
    {
        public static PirateRoulette Instance;

        Player Bomb = null;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
            Exiled.Events.Handlers.Scp049.StartingRecall += OnStartingRecall;
            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(2f);
            var scp = Player.List.ToList()[UnityEngine.Random.Range(0, Player.List.Count())];
            var bomb = Player.List.Where(x => x != scp).ToList()[UnityEngine.Random.Range(0, Player.List.Count())];
            foreach (var p in Player.List)
            {
                if (p == scp)
                {
                    p.Role.Set(RoleTypeId.Scp049, Exiled.API.Enums.SpawnReason.ForceClass, RoleSpawnFlags.None);
                    Timing.CallDelayed(0.25f, () =>
                    {
                        p.Position = RoleTypeId.Tutorial.GetRandomSpawnLocation().Position;
                    });
                }
                else
                {
                    p.Role.Set(RoleTypeId.ClassD);
                    Timing.CallDelayed(0.25f, () =>
                    {
                        p.Position = RoleTypeId.ChaosConscript.GetRandomSpawnLocation().Position;
                        if (p == bomb)
                        {
                            Bomb = bomb;
                            p.ShowHint($"<size=25>당신이 <color=#FA5858>폭탄</color>입니다.</size>\n<size=23>술래가 당신을 잡도록 유도해보세요.</size>\n", 20);
                        }
                    });
                }
            }
            yield return Timing.WaitForSeconds(10f);
            foreach (var p in Player.List)
            {
                if (p == scp)
                {
                    p.Position = RoleTypeId.ChaosConscript.GetRandomSpawnLocation().Position;
                }
                else
                {
                    p.EnableEffect(Exiled.API.Enums.EffectType.Ensnared);
                }
            }

            Timing.RunCoroutine(EndSequence());
        }

        public IEnumerator<float> EndSequence()
        {
            for (; ; )
            {
                yield return Timing.WaitForSeconds(0.5f);
                if (Player.List.Where(x => x.Role == RoleTypeId.ClassD && x != Bomb).Count() == 0)
                {
                    Bomb.Role.Set(RoleTypeId.Spectator, Exiled.API.Enums.SpawnReason.ForceClass, RoleSpawnFlags.None);
                    yield break;
                }
            }
        }

        public void OnReceivingEffect(Exiled.Events.EventArgs.Player.ReceivingEffectEventArgs ev)
        {
            if (ev.Intensity <= 0) return;
            if (!(ev.Effect is CardiacArrest ca)) return;
            if (ev.Player == Bomb)
            {
                foreach (var cdp in Player.List.Where(x => x.Role == RoleTypeId.ClassD))
                {
                    cdp.DisableAllEffects();
                    cdp.IsGodModeEnabled = true;
                }
                var scp = Player.Get(ca._attacker.Hub);
                scp.Health = 1;
                var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, ev.Player);
                g.FuseTime = 0f;
                g.SpawnActive(ev.Player.Position, ev.Player);
            }
            else
            {
                ev.Player.Hurt(new Scp049DamageHandler(ca._attacker.Hub, 32767, Scp049DamageHandler.AttackType.Instakill));
            }
            ev.IsAllowed = false;
        }

        public void OnStartingRecall(Exiled.Events.EventArgs.Scp049.StartingRecallEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnRespawningTeam(Exiled.Events.EventArgs.Server.RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnInteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
