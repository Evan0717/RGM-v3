using DAONTFT.Core.TFT;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using Mirror;

using NetworkManagerUtils.Dummies;
using PlayerRoles;
using RemoteAdmin;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UserSettings.ServerSpecific;
using static RGM.Variables.Variable;

namespace RGM.Modes;

[Mode(ModeCategory.Private, ModeInfo.Lock, ModeType.TFT)]
public class TFT : Mode
{
    public override string Name => "전략적 팀 전투";
    public override string Description => "전략적인 빌드를 구성하여 팀원을 승리로 이끄십시오.";
    public override string Detail =>
"""
증강은 한 사람당 총 3개를 확보할 수 있으며,
처음 라운드 시작시 40초 후에,
그 다음 240초마다 지급됩니다.

10% 확률로 "경쟁전" 모드가 설치됩니다.
""";
    public override string Color => "ffd700";

    CoroutineHandle _onModeStarted;
    CoroutineHandle _upgradeTimerLoop;
    readonly Dictionary<string, float> _upgradeRemainingTimes = new();
    readonly HashSet<string> _playersWaitingRespawn = new();
    readonly List<TFTAbilityLevel> _upgradeLevelSequence = new();
    int _upgradeWaitTime = 240;
    bool _isUpgradeTimerReady;
    const bool DebugUpgradeTimer = true;

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

        Exiled.Events.Handlers.Player.Verified += OnVerified;
        Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
        Exiled.Events.Handlers.Player.Died += OnDied;
        Exiled.Events.Handlers.Player.Spawned += OnSpawned;

        _onModeStarted = Timing.RunCoroutine(OnModeStarted());
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

        Exiled.Events.Handlers.Player.Verified -= OnVerified;
        Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
        Exiled.Events.Handlers.Player.Died -= OnDied;
        Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

        Timing.KillCoroutines(_onModeStarted);
        Timing.KillCoroutines(_upgradeTimerLoop);

        _upgradeRemainingTimes.Clear();
        _playersWaitingRespawn.Clear();
        _upgradeLevelSequence.Clear();
        _isUpgradeTimerReady = false;
    }

    public IEnumerator<float> OnModeStarted()
    {
        if (Random.Range(1, 11) == 1)
            Tools.TryInstallMode(ModeType.Rank);

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var abilityAttribute = type.GetCustomAttribute<TFTAbilityAttribute>();

            if (abilityAttribute == null)
                continue;

            if (!typeof(TFTAbility).IsAssignableFrom(type))
                continue;

            DAONTFT.Core.Variables.Base.TFTAbilities.Add(abilityAttribute.Type, new TFTAbilityData
            {
                Type = type,
                Name = abilityAttribute.Name,
                Description = abilityAttribute.Description,
                Emoji = abilityAttribute.Emoji,
                Level = abilityAttribute.Level,
                Category = abilityAttribute.Category,
                Point = abilityAttribute.Point,
                TFTAbilityType = abilityAttribute.Type,
            });
        }

        QueryProcessor.DotCommandHandler.RegisterCommand(new DAONTFT.Core.Commands.ClientCommands.basicfeatures.SelectTFTFirst());
        QueryProcessor.DotCommandHandler.RegisterCommand(new DAONTFT.Core.Commands.ClientCommands.basicfeatures.SelectTFTSecond());
        QueryProcessor.DotCommandHandler.RegisterCommand(new DAONTFT.Core.Commands.ClientCommands.basicfeatures.RerollTFTFirst());
        QueryProcessor.DotCommandHandler.RegisterCommand(new DAONTFT.Core.Commands.ClientCommands.basicfeatures.RerollTFTSecond());

        foreach (var player in PlayerManager.List)
            DAONTFT.Core.EventArgs.PlayerEvents.Verified(player);

        TFTSetting.Init();

        ServerSpecificSettingsSync.ServerOnSettingValueReceived += TFTSetting.OnSSInput;

        // --------------------------------------------------

        MultiBroadcast.API.MultiBroadcast.ClearAllBroadcasts();

        GlobalPlayer.TryPlay("게임 시작", 2);

        Round.IsLocked = true;

        Dictionary<Player, RoleTypeId> role = new();

        var encounter = DAONTFT.Core.Variables.Base.Encounters.GetRandomValue();
        DAONTFT.Core.Variables.Base.Encounter = encounter.Value.Item1;

        if (encounter.Value.Item1 != RoleTypeId.None)
        {
            Player dummy = Player.Get(DummyUtils.SpawnDummy(encounter.Key));
            dummy.Role.Set(encounter.Value.Item1);
            dummy.Health = 99999;
            dummy.Scale = new Vector3(5, 5, 5);
            dummy.Position = new Vector3(139.8427f, 335.6814f, 67.04181f);

            Timing.CallDelayed(11, () =>
            {
                NetworkServer.Destroy(dummy.GameObject);
            });
        }

        foreach (var player in PlayerManager.List)
        {
            role.Add(player, player.Role.Type);

            player.Role.Set(RoleTypeId.Tutorial);
            player.Position = new Vector3(137.8167f, 304.3213f, 71.88593f);
            player.AddEffect(EffectType.NightVision, 50);

            Timing.CallDelayed(1, () =>
            {
                player.AddBroadcast(10, $"<size=25>{encounter.Value.Item2}</size>");
            });
        }

        Timing.CallDelayed(11, () =>
        {
            foreach (var player in Player.List)
            {
                if (role.ContainsKey(player))
                    player.Role.Set(role[player]);

                else
                    player.Role.Set(RoleTypeId.ClassD);
            }

            try
            {
                if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.ChaosRepressor)
                {
                    foreach (var player in Player.List)
                        player.AddItem(Tools.EnumToList<ItemType>().Where(x => x.IsWeapon()).GetRandomValue());
                }

                if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.ChaosMarauder)
                {
                    foreach (var player in Player.List)
                        player.AddItem(Random.Range(1, 3) == 1 ? ItemType.GrenadeFlash : ItemType.GrenadeHE);
                }

                if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.ChaosConscript)
                {
                    foreach (var player in Player.List)
                        player.AddItem(Tools.EnumToList<ItemType>().Where(x => x.ToString().Contains("SCP")).GetRandomValue());
                }

                if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.ChaosRifleman)
                {
                    foreach (var player in Player.List)
                        player.AddItem(Tools.EnumToList<ItemType>().GetRandomValue());
                }
            }
            catch { }

            Round.IsLocked = false;
        });

        // --------------------------------------------------
        /*
         * 증강 지급 시간 로직 변경
         *
         * 새로 리스폰하는 유저들은 지급 시간을 초기화하도록 변경.
         * 즉, 유저마다 증강 지급 시간을 개별로 적용함.
         *
         * 기존 유저는 시작 40초 후 획득, 이후 240(또는 다른 시간)초마다 획득
         * 중간에 리스폰 한 유저는 리스폰 한 시점 부터 40초 후 획득, 이후 240(또는 다른 시간)초마다 획득
         */
        int getTime()
        {
            if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.ClassD)
                return 120;

            if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.Scientist)
                return 60;

            if (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.FacilityGuard)
                return 180;
            
            return 240;
        }

        _upgradeWaitTime = getTime();
        _isUpgradeTimerReady = true;
        _upgradeTimerLoop = Timing.RunCoroutine(UpgradeTimerLoop());
        DebugBroadcast($"타이머 루프 시작 / 간격: {_upgradeWaitTime}초 / 대상: {PlayerManager.List.Count()}명");

        foreach (var player in PlayerManager.List)
            StartUpgradeTimer(player);

        yield break;
    }

    void StartUpgradeTimer(Player player)
    {
        string key = GetPlayerKey(player);
        if (key == null)
        {
            DebugBroadcast($"타이머 시작 실패 / key null / player: {player?.Nickname ?? "null"}");
            return;
        }

        _playersWaitingRespawn.Remove(key);
        _upgradeRemainingTimes[key] = 40;
        DebugBroadcast($"타이머 시작 / {player.Nickname} / 40초", player);
    }

    void StopUpgradeTimer(Player player)
    {
        string key = GetPlayerKey(player);
        if (key == null)
        {
            DebugBroadcast($"타이머 중지 실패 / key null / player: {player?.Nickname ?? "null"}");
            return;
        }

        _upgradeRemainingTimes.Remove(key);
        DebugBroadcast($"타이머 중지 / {player.Nickname}", player);
    }

    IEnumerator<float> UpgradeTimerLoop()
    {
        while (_isUpgradeTimerReady)
        {
            yield return Timing.WaitForSeconds(1);
            DebugBroadcast($"루프 tick / 등록 타이머: {_upgradeRemainingTimes.Count}명");

            foreach (var key in _upgradeRemainingTimes.Keys.ToList())
            {
                Player player = GetPlayerByKey(key);

                if (!CanReceiveUpgrade(player))
                {
                    DebugBroadcast($"타이머 제거 / {key} / 사유: {GetCannotReceiveReason(player)}", player);
                    _upgradeRemainingTimes.Remove(key);
                    continue;
                }

                _upgradeRemainingTimes[key] -= 1;
                int remaining = Mathf.CeilToInt(_upgradeRemainingTimes[key]);

                if (remaining % 10 == 0 || remaining <= 5)
                    DebugBroadcast($"타이머 진행 / {player.Nickname} / 남은 시간: {remaining}초", player);

                if (_upgradeRemainingTimes[key] > 0)
                    continue;

                TFTAbilityLevel level = GetUpgradeLevel(player.GetAbilities().Count());
                DebugBroadcast($"증강 지급 시도 / {player.Nickname} / 차수: {player.GetAbilities().Count() + 1} / 등급: {level}", player);

                bool isUpgradeStarted;

                try
                {
                    isUpgradeStarted = TFTBattle.StartUpgrade(new List<Player> { player }, level);
                }
                catch (System.Exception e)
                {
                    isUpgradeStarted = false;
                    DebugBroadcast($"증강 지급 예외 / {player.Nickname} / {e.GetType().Name}: {e.Message}", player);
                }

                if (isUpgradeStarted)
                {
                    _upgradeRemainingTimes[key] = _upgradeWaitTime;
                    DebugBroadcast($"증강 지급 완료 / {player.Nickname} / 다음 대기: {_upgradeWaitTime}초", player);
                }
                else
                {
                    _upgradeRemainingTimes[key] = 5;
                    DebugBroadcast($"증강 지급 실패 / {player.Nickname} / 선택지 없음 또는 시작 실패 / 5초 후 재시도", player);
                }
            }
        }
    }

    TFTAbilityLevel GetUpgradeLevel(int upgradeIndex)
    {
        while (_upgradeLevelSequence.Count <= upgradeIndex)
            _upgradeLevelSequence.Add(TFTBattle.GetRandomAbilityLevel());

        return _upgradeLevelSequence[upgradeIndex];
    }

    bool CanReceiveUpgrade(Player player)
    {
        return player != null &&
               player.IsAlive &&
               !player.IsNPC &&
               TFTBattle.GetAbilities(player).Count() < (DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.Scp0492 ? 4 : 3);
    }

    string GetCannotReceiveReason(Player player)
    {
        if (player == null)
            return "player null";

        if (!player.IsAlive)
            return "not alive";

        if (player.IsNPC)
            return "npc";

        int maxAbilityCount = DAONTFT.Core.Variables.Base.Encounter == RoleTypeId.Scp0492 ? 4 : 3;

        if (TFTBattle.GetAbilities(player).Count() >= maxAbilityCount)
            return $"max abilities ({TFTBattle.GetAbilities(player).Count()}/{maxAbilityCount})";

        return "unknown";
    }

    string GetPlayerKey(Player player)
    {
        if (player == null || player.IsNPC || string.IsNullOrEmpty(player.UserId))
            return null;

        return player.UserId;
    }

    Player GetPlayerByKey(string key)
    {
        return PlayerManager.List.FirstOrDefault(x => !x.IsNPC && x.UserId == key);
    }

    void OnVerified(VerifiedEventArgs ev)
    {
        DAONTFT.Core.EventArgs.PlayerEvents.Verified(ev.Player);
        DebugBroadcast($"Verified / {ev.Player.Nickname} / ready: {_isUpgradeTimerReady} / alive: {ev.Player.IsAlive}", ev.Player);

        if (_isUpgradeTimerReady && ev.Player.IsAlive)
            StartUpgradeTimer(ev.Player);
    }

    void OnChangingRole(ChangingRoleEventArgs ev)
    {
        DebugBroadcast($"ChangingRole / {ev.Player.Nickname} / oldDead: {ev.Player.IsDead} / new: {ev.NewRole} / newDead: {ev.NewRole.IsDead()}", ev.Player);

        if (ev.Player.IsDead || ev.NewRole.IsDead() || TFTBattle.GetAbilities(ev.Player).Count() == 0)
        {
            if (ev.NewRole.IsDead())
            {
                string key = GetPlayerKey(ev.Player);
                if (key != null)
                    _playersWaitingRespawn.Add(key);

                StopUpgradeTimer(ev.Player);
            }

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                TFTBattle.Reset(ev.Player);
            });
        }
    }

    void OnDied(DiedEventArgs ev)
    {
        string key = GetPlayerKey(ev.Player);
        if (key == null)
        {
            DebugBroadcast($"Died / key null / {ev.Player.Nickname}", ev.Player);
            return;
        }

        _playersWaitingRespawn.Add(key);
        DebugBroadcast($"Died / {ev.Player.Nickname} / respawn 대기 등록", ev.Player);
        StopUpgradeTimer(ev.Player);
    }

    void OnSpawned(SpawnedEventArgs ev)
    {
        string key = GetPlayerKey(ev.Player);
        DebugBroadcast($"Spawned / {ev.Player.Nickname} / key: {key ?? "null"} / ready: {_isUpgradeTimerReady} / locked: {Round.IsLocked} / waiting: {(key != null && _playersWaitingRespawn.Contains(key))} / hasTimer: {(key != null && _upgradeRemainingTimes.ContainsKey(key))}", ev.Player);

        if (key == null || !_isUpgradeTimerReady || Round.IsLocked)
            return;

        if (!_playersWaitingRespawn.Remove(key) && _upgradeRemainingTimes.ContainsKey(key))
            return;

        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            StartUpgradeTimer(ev.Player);
        });
    }

    void DebugBroadcast(string message, Player player = null)
    {
        if (!DebugUpgradeTimer)
            return;

        string text = $"<size=18><color=#00ffff>[TFT DEBUG]</color> {message}</size>";

        if (player != null)
        {
            player.AddBroadcast(3, text);
            return;
        }

        foreach (var target in PlayerManager.List.Where(x => !x.IsNPC))
            target.AddBroadcast(3, text);
    }

    void OnRoundEnded(RoundEndedEventArgs ev)
    {
        IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

        if (players.Count() == 1)
            Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 5));

        else if (players.Count() > 1)
            Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 1));
    }
}
