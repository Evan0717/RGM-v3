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

namespace RGM.Modes
{
    public class WhoamI
    {
        public static WhoamI Instance;

        public Dictionary<Player, PlayerInfo> PlayersInfo = new Dictionary<Player, PlayerInfo>();

        public class PlayerInfo
        {
            public RoleTypeId RoleType { get; set; }
            public float MaxHealth { get; set; }
            public float Health { get; set; }
            public IEnumerable<StatusEffectBase> ActiveEffects { get; set; }
            public IReadOnlyCollection<Item> Items { get; set; }
            public Item CurrentItem { get; set; }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        }

        public void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            yield return Timing.WaitForSeconds(1f);

            while (true)
            {
                try
                {
                    foreach (var player in Player.List.Where(x => x.IsAlive))
                    {
                        PlayersInfo.Add(player, new PlayerInfo
                        {
                            RoleType = player.Role.Type,
                            MaxHealth = player.MaxHealth,
                            Health = player.Health,
                            ActiveEffects = player.ActiveEffects.ToList(),
                            Items = player.Items.ToList(),
                            CurrentItem = player.CurrentItem,
                            Position = new Vector3(player.Position.x, player.Position.y, player.Position.z)
                        });
                    }

                    foreach (var player in PlayersInfo.Keys.ToList())
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

                        player.Position = new Vector3(PlayersInfo[p].Position.x, PlayersInfo[p].Position.y, PlayersInfo[p].Position.z);

                        if (PlayersInfo.ContainsKey(p))
                            PlayersInfo.Remove(p);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForSeconds(60f);
            }
        }
    }
}
