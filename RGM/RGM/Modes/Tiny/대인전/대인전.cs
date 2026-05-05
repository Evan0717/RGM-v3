using Decals;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using ProjectMER.Features;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RGM.RGM.Modes.Tiny.대인전
{
    public class 대인전
    {
        public static void Start(List<Player> players = null)
        {
            if (players == null)
                players = Player.List.Where(x => !x.IsNPC).ToList();

            players.ShuffleList();

            if (players.Count % 2 != 0)
            {
                Player player = players.Last();

                player.Role.Set(RoleTypeId.Spectator);
                players.Remove(player);

                Map.Broadcast(10, $"쪽수가 맞지 않아 이번엔 {player.DisplayNickname}님이 매치에서 제외되었습니다.");
            }

            int half = players.Count / 2;
            var team1 = players.Take(half).ToList();
            var team2 = players.Skip(half).ToList();

            int originalHalf = int.Parse(half.ToString());
            var originalTeam1 = team1.ToList();
            var originalTeam2 = team2.ToList();
            Dictionary<Player, (int, int)> playerDict = new();
            List<ItemType> items = new()
            {
                ItemType.GunE11SR,
                ItemType.GunAK
            };
            ItemType item = items.GetRandomValue();

            foreach (var ply in team1)
            {
                ply.Role.Set(RoleTypeId.ClassD);
                ply.AddItem(item);
                ply.AddItem(ItemType.ArmorCombat);
                ply.Position = new Vector3(150.292969f, -99.04036f, 59.80078f);
                ply.AddEffect(EffectType.Ensnared, 1, 10);
            }

            foreach (var ply in team2)
            {
                ply.Role.Set(RoleTypeId.Scientist);
                ply.ClearInventory();
                ply.AddItem(item);
                ply.AddItem(ItemType.ArmorCombat);
                ply.Position = new Vector3(180.074219f, -99.04036f, 75.16016f);
                ply.AddEffect(EffectType.Ensnared, 1, 10);
            }

            Map.CleanAllItems();

            Server.ExecuteCommand("/el l all");

            Tools.MessageTranslated("10 . 9 . 8 . 7 . 6 . 5 . 4 . 3 . 2 . 1", "대회 모의전이 곧 시작합니다, 준비하세요!");

            IEnumerator<float> Timeout()
            {
                yield return Timing.WaitForSeconds(180);

                Map.Broadcast(10, "3분 경과");

                while (true)
                {
                    foreach (var ply in Player.List.Where(x => x.IsAlive))
                        ply.Hurt(1);

                    yield return Timing.WaitForSeconds(1);
                }
            }

            var timeout = Timing.RunCoroutine(Timeout());

            void OnShot(ShotEventArgs ev)
            {
                ev.Player.AddAmmo(ev.Firearm.AmmoType, 1);
            }

            void OnHurt(HurtEventArgs ev)
            {
                if (!playerDict.ContainsKey(ev.Attacker))
                    playerDict.Add(ev.Attacker, (0, 0));

                playerDict[ev.Attacker] = (playerDict[ev.Attacker].Item1 + (int)ev.Amount, playerDict[ev.Attacker].Item2);
            }

            void OnDied(DiedEventArgs ev)
            {
                Map.CleanAllItems();

                if (!playerDict.ContainsKey(ev.Attacker))
                    playerDict.Add(ev.Attacker, (0, 0));

                playerDict[ev.Attacker] = (playerDict[ev.Attacker].Item1, playerDict[ev.Attacker].Item2 + 1);

                if (team1.Contains(ev.Player))
                    team1.Remove(ev.Player);
                else if (team2.Contains(ev.Player))
                    team2.Remove(ev.Player);

                void end(string endMessage)
                {
                    Tools.MessageTranslated(endMessage, endMessage);

                    Timing.KillCoroutines(timeout);

                    Exiled.Events.Handlers.Player.Shot -= OnShot;
                    Exiled.Events.Handlers.Player.Hurt -= OnHurt;
                    Exiled.Events.Handlers.Player.Died -= OnDied;

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<size=30><b>[ 매치 종료 - 딜량 순위 ]</b></size>");

                    sb.AppendLine("\n<color=#FF8E00><b>[ D계급 팀 ]</b></color>");
                    foreach (var p in originalTeam1.OrderByDescending(x => playerDict.ContainsKey(x) ? playerDict[x].Item1 : 0))
                    {
                        var stats = playerDict.ContainsKey(p) ? playerDict[p] : (0, 0);
                        sb.AppendLine($"{p.DisplayNickname} - <color=red>딜: {stats.Item1}</color> / <color=yellow>킬: {stats.Item2}</color>");
                    }

                    sb.AppendLine("\n<color=#FFFF7C><b>[ 과학자 팀 ]</b></color>");
                    foreach (var p in originalTeam2.OrderByDescending(x => playerDict.ContainsKey(x) ? playerDict[x].Item1 : 0))
                    {
                        var stats = playerDict.ContainsKey(p) ? playerDict[p] : (0, 0);
                        sb.AppendLine($"{p.DisplayNickname} - <color=red>딜: {stats.Item1}</color> / <color=yellow>킬: {stats.Item2}</color>");
                    }

                    foreach (var ply in Player.List)
                        ply.AddHint("앙", $"{sb.ToString()}\n\n\n\n\n", 20);

                    Timing.CallDelayed(10, () =>
                    {
                        foreach (var ply in Player.List)
                        {
                            ply.Role.Set(RoleTypeId.Tutorial);
                            ply.AddItem(ItemType.GunAK);
                            ply.AddItem(ItemType.GunE11SR);
                        }

                        Server.ExecuteCommand("/close **");
                        Map.CleanAllRagdolls();
                        Map.Clean(DecalPoolType.Bullet);
                        Map.Clean(DecalPoolType.Blood);

                        MapUtils.LoadMap("대인전대회모의전");
                    });
                }

                if (team1.Count == 0)
                    end("과학자 팀 승리!");

                else if (team2.Count == 0)
                    end("D계급 팀 승리!");
            }

            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
            Exiled.Events.Handlers.Player.Died += OnDied;
        }
    }
}
