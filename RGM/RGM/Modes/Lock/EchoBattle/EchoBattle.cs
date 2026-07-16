using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using System.Collections.Generic;
using System.Linq;
using UserSettings.ServerSpecific;

namespace RGM.Modes;

[Mode(ModeCategory.Private, ModeInfo.Lock, ModeType.EchoBattle)]
public class EchoBattle : Mode
{
    public override string Name => "에코 전투";
    public override string Description => "Echo를 장착하고 강화하여 전투하세요.";
    public override string Detail =>
"""
Echo는 메인 1개 + 부가 4개까지 장착할 수 있습니다. (합산 Cost 12 이하)
메인 슬롯 Echo의 액티브 스킬은 [ALT](Noclip) 키로 사용합니다.
모든 능력은 스폰 후 30초 뒤에 적용됩니다.

주의: Echo를 바꾼 뒤에는 바로 아래의 '메인 스탯'도
'자동 (Echo 기본)' 또는 원하는 스탯으로 다시 선택하세요.
(Echo만 바꾸면 메인 스탯 UI가 이전 값으로 남을 수 있습니다.)

Quest (반복)
• 30초 생존 → 80 XP
• 적에게 80 데미지 → 50 XP
• 40 데미지 받기 → 50 XP
• SCP 아이템 획득 → 200 XP
• 적 1명 처치 → 80 XP
• SCP로 적 1회 타격 → 15 XP
• SCP 격리(049-2 제외) → 1200 XP
• SCP-049-2 처치 → 150 XP

[ESC] -> [Settings] -> [Server-specific] 하단부에서 설정을 변경하세요.
""";
    public override string Color => "0077b6";
    public override string Author => "Denia's First Project";

    /// <summary>테스트용: true면 RoundLock + AFK 추방 방지를 켭니다.</summary>
    const bool SoloTestMode = true;

    static readonly List<RoleTypeId> ScpSpawnPoolWithout079 =
    [
        RoleTypeId.Scp049,
        RoleTypeId.Scp096,
        RoleTypeId.Scp106,
        RoleTypeId.Scp173,
        RoleTypeId.Scp939,
    ];

    CoroutineHandle _onModeStarted;
    readonly Dictionary<Player, CoroutineHandle> _hintHandles = new();
    readonly Dictionary<Player, CoroutineHandle> _applyHandles = new();

    public override void OnEnabled()
    {
        if (SoloTestMode)
            Round.IsLocked = true;

        EchoBattleCore.RegisterEchoes();
        ExclusiveWeaponCore.RegisterWeapons();

        Exiled.Events.Handlers.Player.Verified += OnVerified;
        Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        Exiled.Events.Handlers.Player.Hurting += EchoStats.OnHurting;
        Exiled.Events.Handlers.Player.Healing += EchoStats.OnHealing;
        if (SoloTestMode)
            Exiled.Events.Handlers.Player.Kicking += OnKicking;
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

        EchoQuest.Register();
        ExclusiveWeaponQuest.Register();
        EchoSetting.Init();
        ServerSpecificSettingsSync.ServerOnSettingValueReceived += EchoSetting.OnSSInput;

        _onModeStarted = Timing.RunCoroutine(OnModeStarted());
    }

    public override void OnDisabled()
    {
        if (SoloTestMode)
            Round.IsLocked = false;

        Exiled.Events.Handlers.Player.Verified -= OnVerified;
        Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
        Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
        Exiled.Events.Handlers.Player.Hurting -= EchoStats.OnHurting;
        Exiled.Events.Handlers.Player.Healing -= EchoStats.OnHealing;
        if (SoloTestMode)
            Exiled.Events.Handlers.Player.Kicking -= OnKicking;
        Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

        ServerSpecificSettingsSync.ServerOnSettingValueReceived -= EchoSetting.OnSSInput;
        EchoQuest.Unregister();
        ExclusiveWeaponQuest.Unregister();

        Timing.KillCoroutines(_onModeStarted);

        foreach (var handle in _hintHandles.Values)
            Timing.KillCoroutines(handle);
        _hintHandles.Clear();

        foreach (var handle in _applyHandles.Values)
            Timing.KillCoroutines(handle);
        _applyHandles.Clear();

        foreach (var player in Player.List.ToList())
        {
            EchoQuest.ClearPlayer(player);
            EchoGrowth.ClearPending(player);
            ExclusiveWeaponGrowth.ClearPending(player);
            ExclusiveWeaponCore.ClearAll(player);
            EchoBattleCore.Reset(player);
        }

        EchoInfo.PlayerLoadouts.Clear();
        EchoInfo.PlayerEchoes.Clear();
        EchoInfo.PlayerStats.Clear();
        EchoInfo.PlayerShowHints.Clear();
        EchoInfo.PlayerBaseMaxHealth.Clear();
        EchoInfo.PlayerBaseMaxHs.Clear();
        EchoInfo.PlayerPassiveEffects.Clear();
        EchoInfo.Echoes.Clear();
        ExclusiveWeaponInfo.Weapons.Clear();
        ExclusiveWeaponInfo.PlayerWeapons.Clear();
        ExclusiveWeaponInfo.PlayerProgress.Clear();
    }

    IEnumerator<float> OnModeStarted()
    {
        foreach (var p in Player.List)
        {
            ReplaceScp079(p);
            Verified(p);
        }

        for (int i = 0; i < EchoInfo.InitialApplyDelaySeconds; i++)
        {
            foreach (var player in Player.List)
            {
                player.AddBroadcast(1,
                    $"<size=30>Echo 적용까지 <b>{EchoInfo.InitialApplyDelaySeconds - i}</b>초</size>\n" +
                    $"<size=21>[ESC] -> [Settings] -> [Server-specific]ㅣEcho + 메인 스탯을 선택하세요.</size>\n" +
                    $"<size=20><color=#ffcc66>Echo를 바꾼 뒤에는 대응 메인 스탯을 임의로 고른 뒤 다시 원하는 값으로 고르세요.</color></size>\n" +
                    $"<size=19><color=#ffcc66>Echo의 Cost는 총합 12를 넘을 수 없습니다.</color></size>");

                player.AddEffect(EffectType.Ensnared, 1, 1);
                player.AddEffect(EffectType.HeavyFooted, 100, 1);
                player.AddEffect(EffectType.Blinded, 55, 1);
            }

            yield return Timing.WaitForSeconds(1);
        }

        Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;

        foreach (var player in EchoInfo.PlayerLoadouts.Keys.ToList())
        {
            if (player is null || !player.IsAlive)
                continue;

            player.ClearEffect();
            player.AddEffect(EffectType.FogControl, 1);
            EchoBattleCore.ApplyLoadout(player);
        }
    }

    void OnVerified(VerifiedEventArgs ev) => Verified(ev.Player);

    void OnSpawned(SpawnedEventArgs ev) => ReplaceScp079(ev.Player);

    static void ReplaceScp079(Player player)
    {
        if (player?.Role.Type != RoleTypeId.Scp079)
            return;

        player.Role.Set(Tools.GetRandomValue(ScpSpawnPoolWithout079));
    }

    void Verified(Player player)
    {
        if (!_hintHandles.ContainsKey(player))
            _hintHandles[player] = Timing.RunCoroutine(EchoBattleCore.HintDisplay(player));

        if (!EchoInfo.PlayerLoadouts.ContainsKey(player))
            EchoInfo.PlayerLoadouts[player] = new EchoLoadout();

        if (!EchoInfo.PlayerShowHints.ContainsKey(player))
            EchoInfo.PlayerShowHints[player] = true;
    }

    void OnChangingRole(ChangingRoleEventArgs ev)
    {
        if (_applyHandles.TryGetValue(ev.Player, out var old))
            Timing.KillCoroutines(old);

        EchoQuest.StopSurviveTracking(ev.Player);
        EchoGrowth.ClearPending(ev.Player);
        ExclusiveWeaponGrowth.ClearPending(ev.Player);
        EchoBattleCore.Reset(ev.Player);

        if (!ev.NewRole.IsAlive())
            return;

        // ChangingRole 직후엔 아직 IsAlive=false인 프레임이 있어 ApplyAfterDelay가 즉시 종료될 수 있음
        // → 한 프레임 뒤, 실제 스폰 완료를 확인한 다음 카운트다운 시작
        Player player = ev.Player;
        RoleTypeId newRole = ev.NewRole;
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (player == null || !player.IsAlive || player.Role.Type != newRole)
                return;

            if (_applyHandles.TryGetValue(player, out var existing))
                Timing.KillCoroutines(existing);

            _applyHandles[player] = Timing.RunCoroutine(ApplyAfterDelay(player));
        });
    }

    IEnumerator<float> ApplyAfterDelay(Player player)
    {
        // 리스폰 직후 역할이 완전히 잡힐 때까지 대기 (최대 약 2초)
        for (int wait = 0; wait < 40; wait++)
        {
            if (player == null)
                yield break;

            if (player.IsAlive)
                break;

            yield return Timing.WaitForOneFrame;
        }

        if (player == null || !player.IsAlive)
            yield break;

        for (int i = 0; i < EchoInfo.RespawnApplyDelaySeconds; i++)
        {
            if (player == null || !player.IsAlive)
                yield break;

            player.AddBroadcast(1,
                $"<size=30>Echo 적용까지 <b>{EchoInfo.RespawnApplyDelaySeconds - i}</b>초</size>\n" +
                $"<size=21>[ESC] -> [Settings] -> [Server-specific]ㅣEcho + 메인 스탯을 선택하세요.</size>\n" +
                $"<size=20><color=#ffcc66>Echo를 바꾼 뒤에는 대응 메인 스탯을 임의로 고른 뒤 다시 원하는 값으로 고르세요.</color></size>\n" +
                $"<size=19><color=#ffcc66>Echo의 Cost는 총합 12를 넘을 수 없습니다.</color></size>");

            yield return Timing.WaitForSeconds(1);
        }

        if (player != null && player.IsAlive)
            EchoBattleCore.ApplyLoadout(player);
    }

    void OnRoundEnded(RoundEndedEventArgs ev)
    {
        foreach (var player in Player.List.ToList())
        {
            EchoQuest.ClearPlayer(player);
            EchoGrowth.ClearPending(player);
            ExclusiveWeaponGrowth.ClearPending(player);
            ExclusiveWeaponCore.ClearAll(player);
            EchoBattleCore.Reset(player);
        }
        IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

        switch (players.Count())
        {
            case 1:
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 15));
                break;
            case > 1:
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 3));
                break;
        }
    }

    void OnKicking(KickingEventArgs ev)
    {
        if (!SoloTestMode)
            return;

        if (ev.Reason.ToLower().Contains("afk"))
            ev.IsAllowed = false;
    }
}
