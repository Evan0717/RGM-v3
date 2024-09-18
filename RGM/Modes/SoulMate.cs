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

        private Dictionary<Player, Player> soulMates = new Dictionary<Player, Player>();
        private List<Player> waitingPlayers = new List<Player>();

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
            Exiled.Events.Handlers.Player.Healed += OnHealed;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;

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
                        Player.List.ToList().ForEach(x => x.Broadcast(15, $"<size=30><b>{(Player.List.ToList().Where(x => x.IsAlive).Count() == 2 ? "<color=#ffd700>소울메이트</color>" : "<color=#BFFF00>외톨이</color>")}</b>({string.Join(", ", Player.List.ToList().Where(x => x.IsAlive).Select(x => x.DisplayNickname))})의 승리입니다!</size>"));
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
                List<Player> pl = new List<Player>();

                Player.List.CopyTo(pl);
                pl.ShuffleList();

                foreach (var player in pl)
                {
                    if (player.IsAlive)
                    {
                        if (!soulMates.ContainsKey(player))
                            waitingPlayers.Add(player);
                    }
                    else
                    {
                        if (soulMates.ContainsKey(player))
                            soulMates.Remove(player);
                    }
                }

                while (waitingPlayers.Count > 1)
                {
                    Player first = waitingPlayers[0];
                    Player second = waitingPlayers[1];

                    waitingPlayers.Remove(first);
                    waitingPlayers.Remove(second);

                    soulMates.Add(first, second);
                    soulMates.Add(second, first);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> CurrentItemAsync()
        {
            Dictionary<Player, Item> currentItems = new Dictionary<Player, Item>();

            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (currentItems.ContainsKey(player))
                    {
                        if (currentItems[player] != player.CurrentItem)
                        {
                            Player soulMate = soulMates[player];

                            foreach (var Item in soulMate.Items)
                            {
                                if (Item.Type == player.CurrentItem.Type)
                                {
                                    soulMate.CurrentItem = Item;
                                    break;
                                }
                            }
                        }

                        currentItems[player] = player.CurrentItem;
                    }
                    else
                    {
                        currentItems.Add(player, player.CurrentItem);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
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
    }
}
