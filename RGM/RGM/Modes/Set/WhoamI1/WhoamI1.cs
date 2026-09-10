using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;
using CustomPlayerEffects;
using Exiled.API.Extensions;
using RGM.API.Interfaces;
using RGM.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Player;
using RGM.Patches;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.WhoamI1)]
    public class WhoamI1 : Mode
    {
        public override string Name => "나는 누구?";
        public override string Description => "랜덤한 시간대에 다른 플레이어와 몸이 바뀝니다.";
        public override string Detail =>
"""
[] 나는 누구?

<b>[참고]</b>
• <color=red>죽어야 하는 장소에 스폰했을 때 [.자살] 명령어를 입력하지 않으면 제재 대상입니다.</color>

* 게임 시작 10분 뒤 <color=red>자동핵</color>이 작동됩니다.
""";
        public override string Color => "886A08";

        public static WhoamI1 Instance;

        Dictionary<Player, PlayerInfo> PlayersInfo = new();

        private CoroutineHandle _onModeStarted;
        private readonly AutoWarhead _autoWarhead = new(10, 1);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            _onModeStarted = Timing.RunCoroutine(OnModeStarted());
            _autoWarhead.RunCoroutine();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

            Timing.KillCoroutines(_onModeStarted);
            _autoWarhead.KillCoroutine();
        }

        private IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                foreach (var player in PlayerManager.List.Where(x => x.IsAlive).ToList())
                {
                    try
                    {
                        RoleTypeId RoleType = player.Role.Type;
                        float MaxHealth = player.MaxHealth;
                        float Health = player.Health;
                        List<StatusEffectBase> ActiveEffects = player.ActiveEffects.ToList();
                        List<Item> Items = player.Items == null ? new List<Item>() : player.Items.ToList();
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

                        if (!PlayersInfo.ContainsKey(player))
                            PlayersInfo.Add(player, pi);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error on saving player info: {ex}");
                    }
                }

                foreach (var player in PlayersInfo.Keys.ToList())
                {
                    try
                    {
                        Player p = PlayersInfo.Keys.Where(x => x != player).ToList().GetRandomValue();

                        player.Role.Set(PlayersInfo[p].RoleType);
                        player.MaxHealth = PlayersInfo[p].MaxHealth;
                        player.Health = PlayersInfo[p].Health;

                        foreach (var effect in PlayersInfo[p].ActiveEffects.ToList())
                            player.EnableEffect(effect, effect.Intensity, effect.Duration);

                        player.ClearItems();

                        foreach (var item in PlayersInfo[p].Items.ToList())
                            player.AddItem(item.Type);

                        if (PlayersInfo[p].CurrentItem != null)
                        {
                            player.CurrentItem = player.Items.ToList().Find(x => x.Type == PlayersInfo[p].CurrentItem.Type);
                        }

                        player.Position = PlayersInfo[p].Position;

                        if (PlayersInfo.ContainsKey(p))
                            PlayersInfo.Remove(p);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error on setting player info: {ex}");
                    }
                }

                PlayersInfo.Clear();

                yield return Timing.WaitForSeconds(UnityEngine.Random.Range(15, 120));
            }
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            List<Player> players = [.. PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC)];

            switch (players.Count)
            {
                case 1:
                    Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));
                    break;
                case > 1:
                    Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
                    break;
            }
        }

        private void OnSpawned(SpawnedEventArgs ev)
        {
            ev.Player.AddHint("나는 누구?", $"<b>⚠️ 주의하세요</b>, <color=red>죽어야 하는 장소에 스폰했을 때 [.자살] 명령어를 입력하지 않으면 제재 대상입니다.</color>", 10);
        }
    }
}
