using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomPlayerEffects;
using CustomRendering;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using ProjectMER.Features.Objects;
using MEC;
using Mirror;
using MultiBroadcast;
using PlayerRoles;
using UnityEngine;
using Exiled.API.Enums;
using RGM.API.Features;
using Exiled.Events.EventArgs.Server;
using MultiBroadcast.API;
using Respawning;
using static RGM.Variables.ServerManagers;
using ProjectMER.Features;
using ProjectMER.Features.Serializable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.HIDE)]
    class HIDE : Mode
    {
        public override string Name => "HIDE";
        public override string Description => "숨 죽이는 그를 사살하십시오.";
        public override string Detail =>
"""
<color=red>SCP-3114</color>는 피격당하거나 공격하면 투명이 해제됩니다.
또한, 평상시에도 반투명 상태로 존재합니다. 이 점을 잘 이용해보세요!

<b>[Map Credit]</b>
@vlrpfrjs
""";
        public override string Color => "0489B1";

        public static HIDE Instance;

        public List<Player> pl = new List<Player>();
        public Player monster = null;


        public override void OnEnabled()
        {
            Respawn.PauseWaves(); 
            Server.ExecuteCommand($"/el l all");
            Server.ExecuteCommand($"/close **");
            Server.ExecuteCommand($"/lock **");

            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
            Exiled.Events.Handlers.Player.Jumping += OnJumping;

            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(Timer());
        }

        public IEnumerator<float> OnModeStarted()
        {
            MapSchematic map = Tools.LoadMap($"container");

            Player.List.ToList().CopyTo(pl);
            monster = Tools.GetRandomValue(Player.List.ToList());

            try
            {
                Timing.CallDelayed(1f, () =>
                {
                    monster.Role.Set(RoleTypeId.Scp3114);
                    monster.RankName = "MONSTER";
                    monster.RankColor = "red";
                    monster.Position = new Vector3(-0.6015625f, 332.9026f, -32.56641f);
                    Server.ExecuteCommand($"/open ESCAPE_PRIMARY");

                    float health = 11 * Player.List.Count + 3 * Player.List.Count;
                    monster.MaxHealth = health;
                    monster.Health = health;
                    monster.IsUsingStamina = false;
                    monster.EnableEffect(EffectType.MovementBoost, 50);
                    monster.EnableEffect(EffectType.Fade, 222);

                    foreach (var player in Player.List)
                    {
                        if (player != monster)
                        {
                            player.Role.Set(RoleTypeId.NtfPrivate);
                            player.Position = new Vector3(36.61497f, 332.9037f, -69.72147f);
                            for (int i = 1; i < 10; i++)
                                player.AddItem(ItemType.Ammo9x19);
                        }
                    }


                });
            }
            catch (Exception e)
            {
                ServerConsole.AddLog(e.ToString());
            }

            while (true)
            {
                foreach (var obj in map.SpawnedObjects)
                {
                    if (obj.name == "CustomSchematic-MonsterCapsule")
                        obj.Base.Position = monster.Position;
                }

                yield return Timing.WaitForSeconds(0.01f);
            }
        }

        public IEnumerator<float> Timer()
        {
            for (int i = 1; i < 180; i++)
            {
                Player.List.ToList().ForEach(x => x.AddBroadcast(1, $"<size=25><color=#2ECCFA>NTF 승리</color>까지</color> {180 - i}초</size>"));

                yield return Timing.WaitForSeconds(1f);
            }

            foreach (var player in Player.List)
            {
                if (player.IsScp)
                {
                    if (GodModePlayers.Contains(player))
                        GodModePlayers.Remove(player);

                    player.Kill("제한시간이 초과하였습니다.");
                }
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (ev.Attacker.IsScp && ev.DamageHandler.Type != DamageType.Strangled)
                ev.DamageHandler.Damage += 60;
        }

        public void OnHurt(Exiled.Events.EventArgs.Player.HurtEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker == monster)
                ev.Attacker.DisableEffect(EffectType.Invisible);

            if (ev.Player == monster)
                ev.Player.DisableEffect(EffectType.Invisible);
        }

        public IEnumerator<float> OnJumping(Exiled.Events.EventArgs.Player.JumpingEventArgs ev)
        {
            if (ev.Player == monster)
            {
                for (int i = 1; i < 11; i++)
                {
                    if (Physics.Raycast(monster.Position, Vector3.up, out RaycastHit hit, 1, (LayerMask)1))
                        break;

                    ev.Player.Position += new Vector3(0, 0.3f, 0);

                    yield return Timing.WaitForSeconds(0.01f);
                }
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            IEnumerable<Player> players = Player.List.Where(x => x.IsAlive && !x.IsNPC);

            if (players.Count() == 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

            else if (players.Count() > 1)
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
        }
    }
}
