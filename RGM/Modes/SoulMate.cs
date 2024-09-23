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
using MultiBroadcast.API;
using UnityEngine;

namespace RGM.Modes
{
    class SoulMate
    {
        public static SoulMate Instance;

        private Dictionary<Player, Player> soulMates = new Dictionary<Player, Player>();
        private List<Player> waitingPlayers = new List<Player>();

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
            Exiled.Events.Handlers.Player.Healed += OnHealed;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.UsingItemCompleted += OnUsingItemCompleted;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;

            Timing.RunCoroutine(OnModeStarted());
            Timing.RunCoroutine(SoulMateMatching());
            Timing.RunCoroutine(CurrentItemAsync());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (Player.List.ToList().Where(x => x.IsAlive).Count() < 3 && player.Role.Type != PlayerRoles.RoleTypeId.Tutorial)
                    {
                        Player.List.ToList().Where(x => x.IsAlive).ToList().ForEach(x => Server.ExecuteCommand($"/fc {x.Id} Tutorial 1"));
                        Player.List.ToList().ForEach(x => x.AddBroadcast(15, $"<size=30><b>{(Player.List.ToList().Where(x => x.IsAlive).Count() == 2 ? "<color=#ffd700>소울메이트</color>" : "<color=#BFFF00>외톨이</color>")}</b>({string.Join(", ", Player.List.ToList().Where(x => x.IsAlive).Select(x => x.DisplayNickname))})의 승리입니다!</size>"));
                        yield return Timing.WaitForSeconds(100f);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> SoulMateMatching()
        {
            while (true)
            {
                foreach (var sm in soulMates.Keys.ToList())
                {
                    if (soulMates[sm] == null)
                    {
                        Player soulMate = soulMates[sm];

                        soulMates.Remove(soulMate);
                        soulMates.Remove(sm);

                        sm.ShowHint("누군가와의 매칭이 해제되었습니다.", 1.2f);
                    }
                }

                foreach (var player in Player.List)
                {
                    if (player.IsAlive)
                    {
                        if (!soulMates.ContainsKey(player))
                        {
                            if (!waitingPlayers.Contains(player))
                                waitingPlayers.Add(player);

                            player.ShowHint("누군가와 매칭되기를 기다리는 중입니다..", 1.2f);
                        }
                    }
                    else
                    {
                        if (soulMates.ContainsKey(player))
                        {
                            Player soulMate = soulMates[player];

                            soulMates.Remove(soulMate);
                            soulMates.Remove(player);

                            player.ShowHint("누군가와의 매칭이 해제되었습니다.", 1.2f);
                            soulMate.ShowHint("누군가와의 매칭이 해제되었습니다.", 1.2f);
                        }
                    }
                }

                while (waitingPlayers.Count() > 1)
                {
                    Player first = RGM.GetRandomValue(waitingPlayers);
                    Player second = RGM.GetRandomValue(waitingPlayers.Where(x => x != first).ToList());

                    waitingPlayers.Remove(first);
                    waitingPlayers.Remove(second);

                    soulMates.Add(first, second);
                    soulMates.Add(second, first);

                    second.MaxHealth = first.MaxHealth;
                    second.Health = first.Health;

                    second.ClearInventory();
                    foreach (var Item in first.Items)
                        second.AddItem(Item.Type);

                    first.ShowHint("누군가와 새롭게 매칭되었습니다.", 5);
                    second.ShowHint("누군가와 새롭게 매칭되었습니다.", 5);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> CurrentItemAsync()
        {
            Dictionary<Player, Item> currentItems = new Dictionary<Player, Item>();

            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive && soulMates.ContainsKey(x)))
                {
                    if (currentItems.ContainsKey(player))
                    {
                        if (currentItems[player] != player.CurrentItem)
                        {
                            Timing.CallDelayed(0.1f, () =>
                            {
                                Player soulMate = soulMates[player];

                                if (player.CurrentItem == null)
                                    soulMate.CurrentItem = null;

                                else
                                {
                                    foreach (var Item in soulMate.Items)
                                    {
                                        if (Item.Type == player.CurrentItem.Type)
                                        {
                                            soulMate.CurrentItem = Item;
                                            break;
                                        }
                                    }
                                }
                            });
                        };

                        currentItems[player] = player.CurrentItem;
                    }
                    else
                        currentItems.Add(player, player.CurrentItem);
                }

                yield return Timing.WaitForSeconds(0.1f);
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

        public void OnHurt(Exiled.Events.EventArgs.Player.HurtEventArgs ev)
        {
            if (soulMates.ContainsKey(ev.Player))
            {
                Player soulMate = soulMates[ev.Player];

                soulMate.MaxHealth = ev.Player.MaxHealth;
                soulMate.Health = ev.Player.Health;
            }
        }

        public void OnHealed(Exiled.Events.EventArgs.Player.HealedEventArgs ev)
        {
            if (soulMates.ContainsKey(ev.Player))
            {
                Player soulMate = soulMates[ev.Player];

                soulMate.MaxHealth = ev.Player.MaxHealth;
                soulMate.Health = ev.Player.Health;
            }
        }

        public void OnPickingUpItem(Exiled.Events.EventArgs.Player.PickingUpItemEventArgs ev)
        {
            if (soulMates.ContainsKey(ev.Player))
            {
                Player soulMate = soulMates[ev.Player];

                soulMate.AddItem(ev.Pickup.Type);
            }
        }

        public void OnDroppingItem(Exiled.Events.EventArgs.Player.DroppingItemEventArgs ev)
        {
            if (soulMates.ContainsKey(ev.Player))
            {
                Player soulMate = soulMates[ev.Player];

                foreach (var Item in soulMate.Items)
                {
                    if (Item.Type == ev.Item.Type)
                    {
                        soulMate.RemoveItem(Item);
                        break;
                    }
                }
            }
        }

        public void OnUsingItemCompleted(Exiled.Events.EventArgs.Player.UsingItemCompletedEventArgs ev)
        {
            if (soulMates.ContainsKey(ev.Player))
            {
                Player soulMate = soulMates[ev.Player];

                foreach (var Item in soulMate.Items)
                {
                    if (Item.Type == ev.Item.Type)
                    {
                        soulMate.RemoveItem(Item);
                        break;
                    }
                }
            }
        }

        public void OnEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () => 
            {
                if (soulMates.ContainsKey(ev.Player))
                {
                    Player soulMate = soulMates[ev.Player];

                    ev.Player.MaxHealth = soulMate.MaxHealth;
                    ev.Player.Health = soulMate.Health;

                    soulMate.ClearInventory();
                    foreach (var Item in ev.Player.Items)
                        soulMate.AddItem(Item.Type);
                }
            });
        }
    }
}
