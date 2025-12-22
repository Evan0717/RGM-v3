using AdminToys;
using CommandSystem.Commands.RemoteAdmin;
using CustomPlayerEffects;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Scp244;
using Exiled.Events.EventArgs.Server;
using Interactables.Interobjects.DoorUtils;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
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
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.PirateRoulette)]
    public class PirateRoulette : Mode
    {
        public override string Name => "해적 룰렛";
        public override string Description => "폭탄을 잘 추려내야 합니다.";
        public override string Detail =>
"""
운빨 싸움
""";
        public override string Color => "FFBF00";

        public static PirateRoulette Instance;

        Player Bomb = null;

        CoroutineHandle _onModeStarted;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;

            Exiled.Events.Handlers.Scp049.StartingRecall += OnStartingRecall;

            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;

            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.Kicking += OnKicking;

            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.ReceivingEffect -= OnReceivingEffect;

            Exiled.Events.Handlers.Scp049.StartingRecall -= OnStartingRecall;

            Exiled.Events.Handlers.Server.RespawningTeam -= OnRespawningTeam;

            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.Kicking -= OnKicking;

            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Timing.KillCoroutines(_onModeStarted);
        }

        public IEnumerator<float> OnModeStarted()
        {
            var scp = PlayerManager.List.ToList()[UnityEngine.Random.Range(0, PlayerManager.List.Count())];
            var bomb = PlayerManager.List.Where(x => x != scp).ToList()[UnityEngine.Random.Range(0, PlayerManager.List.Count() - 1)];

            foreach (var p in PlayerManager.List)
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
                            p.AddHint("해적 룰렛", $"<size=25>당신이 <color=#FA5858>폭탄</color>입니다.</size>\n<size=23>술래가 당신을 잡도록 유도해보세요.</size>\n", 20);
                        }
                    });
                }
            }
            yield return Timing.WaitForSeconds(5f);
            foreach (var p in PlayerManager.List)
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

            Timing.RunCoroutine(HurtScp049());
            Timing.RunCoroutine(EndSequence());
        }

        public IEnumerator<float> HurtScp049()
        {
            while (Bomb.IsAlive)
            {
                Bomb.Hurt(Bomb.MaxHealth / 150);

                yield return Timing.WaitForSeconds(1);
            }
        }

        public IEnumerator<float> EndSequence()
        {
            for (; ; )
            {
                yield return Timing.WaitForSeconds(0.5f);
                if (PlayerManager.List.Where(x => x.Role == RoleTypeId.ClassD && x != Bomb).Count() == 0)
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
                foreach (var cdp in PlayerManager.List.Where(x => x.Role == RoleTypeId.ClassD))
                {
                    cdp.DisableAllEffects();
                    GodModePlayers.Add(cdp);
                }
                var scp = Player.Get(ca._attacker);
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

        public void OnKicking(Exiled.Events.EventArgs.Player.KickingEventArgs ev)
        {
            if (ev.Reason.ToLower().Contains("afk"))
                ev.IsAllowed = false;
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC).ToList()[0].Role.Type == RoleTypeId.Scp049)
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x.IsAlive).ToList(), 15));

            else
                Timing.RunCoroutine(Tools.SetWinner(PlayerManager.List.Where(x => x.IsAlive).ToList(), 1));
        }
    }
}
