using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API;
using Achievements.Handlers;
using Exiled.API.Enums;
using UnityEngine;
using CustomPlayerEffects;
using MultiBroadcast.API;
using RGM.API.Interfaces;
using RGM.API.Features;
using Exiled.Events.EventArgs.Server;

namespace RGM.Modes
{
    [Mode(ModeCategory.Private, ModeInfo.Set, ModeType.WhoamI)]
    public class WhoamI : Mode
    {
        public override string Name => "나는 누구?";
        public override string Description => "1분마다 진영이 변경됩니다.";
        public override string Detail =>
"""
[] 나는 누구?
""";
        public override string Color => "886A08";

        public static WhoamI Instance;

        public Dictionary<Player, PlayerInfo> PlayersInfo = new Dictionary<Player, PlayerInfo>();

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive))
                {
                    try
                    {
                        RoleTypeId RoleType = player.Role.Type;
                        float MaxHealth = player.MaxHealth;
                        float Health = player.Health;
                        IEnumerable<StatusEffectBase> ActiveEffects = player.ActiveEffects.ToList();
                        IReadOnlyCollection<Item> Items = player.Items == null ? new List<Item>() : player.Items.ToList();
                        Item CurrentItem = player.CurrentItem;
                        Vector3 Position = player.Position;

                        PlayerInfo pi = new PlayerInfo
                        {
                            RoleType = RoleType,
                            MaxHealth = MaxHealth,
                            Health = Health,
                            ActiveEffects = ActiveEffects,
                            Items = Items,
                            CurrentItem = CurrentItem,
                            Position = Position
                        };

                        if (PlayersInfo.ContainsKey(player))
                            PlayersInfo[player] = pi;

                        else
                            PlayersInfo.Add(player, pi);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                foreach (var player in PlayersInfo.Keys.ToList())
                {
                    try
                    {
                        Player p = Tools.GetRandomValue(PlayersInfo.Keys.Where(x => x != player).ToList());

                        player.Role.Set(PlayersInfo[p].RoleType);
                        player.MaxHealth = PlayersInfo[p].MaxHealth;
                        player.Health = PlayersInfo[p].Health;

                        foreach (var effect in PlayersInfo[p].ActiveEffects)
                            player.EnableEffect(effect, effect.Intensity, effect.Duration);

                        player.ClearItems();

                        foreach (var item in PlayersInfo[p].Items)
                            player.AddItem(item.Type);

                        player.CurrentItem = player.Items.ToList().Find(x => x.Type == PlayersInfo[p].CurrentItem.Type);

                        player.Position = PlayersInfo[p].Position;

                        if (PlayersInfo.ContainsKey(p))
                            PlayersInfo.Remove(p);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                PlayersInfo.Clear();

                yield return Timing.WaitForSeconds(60f);
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Timing.RunCoroutine(Tools.SetWinner(Player.List.Where(x => x.IsAlive).ToList(), 1));
        }
    }
}
