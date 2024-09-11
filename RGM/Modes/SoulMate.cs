using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using Mirror;
using UnityEngine;

namespace RGM.Modes
{
    class SoulMate
    {
        public static SoulMate Instance;

        private Dictionary<Player, Player> soulMates;
        private List<Player> waitingPlayers;

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Hurt += OnHurt;
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(10f);

            Timing.RunCoroutine(SoulMateMatching());

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

        public IEnumerator<float> SoulMateMatching()
        {
            soulMates = new Dictionary<Player, Player>();
            waitingPlayers = new List<Player>();

            List<Player> players = Player.List.ToList();

            players.ShuffleList();

            for (int i = 0; i < players.Count; i += 2)
            {
                if (i + 1 < players.Count)
                {
                    soulMates.Add(players[i], players[i + 1]);
                    soulMates.Add(players[i + 1], players[i]);
                }
            }

            yield return Timing.WaitForSeconds(10f);

            while (true)
            {
                foreach (var player in soulMates.Keys.ToList())
                {
                    if (player.IsDead)
                    {
                        Player soulMate = soulMates[player];
                        soulMates.Remove(player);

                        if (soulMate != null && soulMate.IsAlive)
                        {
                            soulMate.Kill($"{player.DisplayNickname}(와)과 {soulMate.DisplayNickname}(은)는 영혼의 단짝이였습니다.");
                            Server.ExecuteCommand($"/cassie_sl <color=red>{player.DisplayNickname}</color>(이)가 영혼의 단짝이였던 <color=#5858FA>{player.DisplayNickname}</color>와(과) <color=#FE2EF7>{soulMate.DisplayNickname}</color>을(를) 사이좋게 하늘로 보냈습니다.");
                        }
                    }
                    else if (player.IsAlive && !soulMates.ContainsKey(player))
                    {
                        waitingPlayers.Add(player);
                    }
                }

                if (waitingPlayers.Count >= 2)
                {
                    Player player1 = waitingPlayers[0];
                    Player player2 = waitingPlayers[1];

                    soulMates.Add(player1, player2);
                    soulMates.Add(player2, player1);

                    waitingPlayers.Remove(player1);
                    waitingPlayers.Remove(player2);
                }

                yield return Timing.WaitForSeconds(10f);
            }
        }

        public void OnHurt(Exiled.Events.EventArgs.Player.HurtEventArgs ev)
        {
            if (soulMates.ContainsKey(ev.Player))
            {
                Player soulMate = soulMates[ev.Player];

                if (soulMate != null && soulMate.IsAlive)
                {
                    soulMate.MaxHealth = ev.Player.MaxHealth;
                    soulMate.Health = ev.Player.Health;
                }
            }
        }

        public void OnChaningItem(Exiled.Events.EventArgs.Player.ChangingItemEventArgs ev)
        {
            if (soulMates.ContainsKey(ev.Player))
            {
                Player soulMate = soulMates[ev.Player];

                if (soulMate != null && soulMate.IsAlive)
                {
                    soulMate.ClearInventory();

                    foreach (Item Item in ev.Player.Items)
                        soulMate.AddItem(Item);
                }
            }
        }
    }
}
