using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace RGM.Modes
{
    class SoulMate
    {
        public static SoulMate Instance;

        private Dictionary<Player, Player> soulMates;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10f);

            soulMates = new Dictionary<Player, Player>();

            List<Player> players = Player.List.ToList();

            players.ShuffleList();

            for (int i = 0; i < players.Count; i += 2)
            {
                if (i + 1 < players.Count)
                {
                    soulMates.Add(players[i], players[i + 1]);
                    soulMates.Add(players[i + 1], players[i]);

                    for (int n = i; n < i + 2; n++)
                        players[n].ShowHint($"당신의 단짝이 존재하나 누군지 모릅니다..", 5);
                }
                else
                {
                    soulMates.Add(players[i], null);

                    players[i].ShowHint($"당신은 <color=#BFFF00>외톨이</color>입니다.\n<color=red>분노</color>로 인해 체력이 2배 상승합니다.", 5);
                    players[i].MaxHealth = players[i].MaxHealth * 2;
                    players[i].Health = players[i].MaxHealth;
                    players[i].Group = new UserGroup { BadgeText = "외톨이", BadgeColor = "lime" };
                }
            }

            yield return Timing.WaitForSeconds(5f);

            while (true)
            {
                foreach (var player in Player.List)
                {
                    /*
                    if (soulMates.ContainsKey(player) && soulMates[player] != null && soulMates[player].IsAlive)
                        player.ShowHint($"당신의 단짝은 <b>{soulMates[player].CurrentRoom.Name}</b>에 있습니다.", 1.2f);
                    */

                    if (Player.List.ToList().Where(x => x.IsAlive).Count() < 3 && player.Role.Type != PlayerRoles.RoleTypeId.Tutorial)
                    {
                        Player.List.ToList().Where(x => x.IsAlive).ToList().ForEach(x => Server.ExecuteCommand($"/fc {x.Id} Tutorial 1"));
                        Player.List.ToList().ForEach(x => x.Broadcast(15, $"<size=30><b>{(Player.List.ToList().Where(x => x.IsAlive).Count() == 2 ? "<color=#ffd700>소울메이트</color>" : "<color=#BFFF00>외톨이</color>")}</b>({string.Join(", ", Player.List.ToList().Where(x => x.IsAlive).Select(x => x.DisplayNickname))})의 승리입니다!</size>"));
                        yield return Timing.WaitForSeconds(100f);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void OnDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            if (soulMates.ContainsKey(ev.Player))
            {
                Player soulMate = soulMates[ev.Player];

                if (soulMate != null && soulMate.IsAlive)
                {
                    soulMate.Kill($"{ev.Player.DisplayNickname}(와)과 {soulMate.DisplayNickname}(은)는 영혼의 단짝이였습니다.");
                    ev.Player.Kill($"{soulMate.DisplayNickname}(와)과 {ev.Player.DisplayNickname}(은)는 영혼의 단짝이였습니다.");
                    Server.ExecuteCommand($"/cassie_sl <color=red>{ev.Attacker.DisplayNickname}</color>(이)가 영혼의 단짝이였던 <color=#5858FA>{ev.Player.DisplayNickname}</color>와(과) <color=#FE2EF7>{soulMate.DisplayNickname}</color>을(를) 사이좋게 하늘로 보냈습니다.");
                }
            }
        }
    }
}
