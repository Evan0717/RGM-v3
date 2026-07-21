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

[ESC] -> [Settings] -> [Server-specific] 하단부에서 설정을 변경하세요.
""";
    public override string Color => "0077b6";
    public override string Author => "Denia's First Project";

    /// <summary>테스트용: true면 RoundLock + AFK 추방 방지를 켭니다.</summary>
    const bool SoloTestMode = true;
    const int InstructionDurationSeconds = 150;
    public const int RoundStartDelaySeconds = InstructionDurationSeconds + EchoInfo.InitialApplyDelaySeconds;
    // Hint의 기본 줄 간격보다 조금 좁게 지정해, 긴 안내문을 자연스럽게 읽을 수 있게 합니다.
    const string InstructionHintFormat = "<voffset=360><align=left><width=900><line-height=85%>{0}</line-height></width></align></voffset>";

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
    readonly Dictionary<Player, RoleTypeId> _rolesBeforeInstruction = new();
    bool _isShowingInstruction;

    public override void OnEnabled()
    {
        if (SoloTestMode)
            Round.IsLocked = true;

        Respawn.PauseWaves();
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
        if (SoloTestMode || _isShowingInstruction)
            Round.IsLocked = false;

        Respawn.ResumeWaves();
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
        _isShowingInstruction = false;
        RestoreRolesAfterInstruction();

        foreach (var handle in _hintHandles.Values)
            Timing.KillCoroutines(handle);
        _hintHandles.Clear();

        foreach (var handle in _applyHandles.Values)
            Timing.KillCoroutines(handle);
        _applyHandles.Clear();

        foreach (var player in Player.List.ToList())
            ClearPlayerState(player, "모드 비활성화");

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
        _isShowingInstruction = true;
        Round.IsLocked = true;

        foreach (var p in Player.List)
        {
            Verified(p);
            SetSpectatorForInstruction(p);
        }

        yield return Timing.WaitForOneFrame;
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstructions()));

        _isShowingInstruction = false;
        RestoreRolesAfterInstruction();
        // 안내 종료 후 일반 에코 전투만 라운드 잠금을 해제합니다.
        // SoloTestMode에서는 테스트가 끝날 때까지 RoundLock을 유지해야 합니다.
        if (!SoloTestMode)
            Round.IsLocked = false;

        // 역할 복구가 완료되어 스폰 상태가 반영될 때까지 한 프레임 대기합니다.
        yield return Timing.WaitForOneFrame;

        foreach (var player in Player.List)
            StartHintDisplay(player);

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

        Respawn.ResumeWaves();
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
        if (_isShowingInstruction)
            SetSpectatorForInstruction(player);

        if (!EchoInfo.PlayerLoadouts.ContainsKey(player))
            EchoInfo.PlayerLoadouts[player] = new EchoLoadout();

        if (!EchoInfo.PlayerShowHints.ContainsKey(player))
            EchoInfo.PlayerShowHints[player] = true;

        if (!_isShowingInstruction)
            StartHintDisplay(player);
    }

    void StartHintDisplay(Player player)
    {
        if (!_hintHandles.ContainsKey(player))
            _hintHandles[player] = Timing.RunCoroutine(EchoBattleCore.HintDisplay(player));
    }

    void SetSpectatorForInstruction(Player player)
    {
        if (player == null || !player.IsConnected)
            return;

        if (!_rolesBeforeInstruction.ContainsKey(player))
            _rolesBeforeInstruction[player] = player.Role.Type;

        if (player.Role.Type != RoleTypeId.Spectator)
            player.Role.Set(RoleTypeId.Spectator, RoleSpawnFlags.None);
    }

    void RestoreRolesAfterInstruction()
    {
        foreach (var entry in _rolesBeforeInstruction.ToList())
        {
            Player player = entry.Key;
            RoleTypeId role = entry.Value;
            if (player == null || !player.IsConnected)
                continue;

            RoleTypeId restoredRole = role == RoleTypeId.Scp079
                ? Tools.GetRandomValue(ScpSpawnPoolWithout079)
                : role;

            if (player.Role.Type == RoleTypeId.Spectator && restoredRole != RoleTypeId.Spectator)
                player.Role.Set(restoredRole);
        }

        _rolesBeforeInstruction.Clear();
    }

    IEnumerator<float> ShowInstructions()
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<align=center><size=28><color=#f90909>주의! 이 모드는 로직이 많이 어렵습니다!</color></size>\n\n" +
            "<size=23>차례대로 보여주는 모드 설명을 필히 읽으시기 바랍니다.</size></align>", 5)));

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<size=27><b>1. 에코 전투 기본 시스템</b></size>\n\n" +
            "<size=21>에코 전투 모드는 부여된 에코를 활용하여 전략적인 전투를 펼칠 수 있는 모드입니다.\n" +
            "이 모드에서는 SCP-079가 스폰되지 않습니다.\n\n" +
            "모든 기능은 [ESC] → [Settings] → [Server-specific] 하단부에서 설정 가능합니다.</size>", 5)));

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<size=27><b>2. 에코 전투 장착/Cost 시스템</b></size>\n\n" +
            "<size=20>에코에는 각각의 등급마다 부여되는 Cost가 있으며, Cost 별 장착할 수 있는 메인 스탯이 부여됩니다.\n" +
            "또한, 에코의 각 Cost 마다 고정적으로 부가 옵션이 부여됩니다.\n" +
            "에코 슬롯은 총 5개가 부여되며, 메인 슬롯 1개와 부가 슬롯 4개로 구성됩니다.\n" +
            "메인 슬롯에 장착한 에코는 액티브 스킬을 사용할 수 있습니다. (기본값: [ALT])\n" +
            "Cost 1 에코는 액티브 스킬이 없습니다.\n\n" +
            "에코 장착에는 최대 Cost 제한이 있으며, 총합 Cost는 12까지 구성 가능합니다.\n" +
            "같은 이름의 에코는 중복 장착이 불가합니다.\n" +
            "각 Cost별 에코는 장착할 수 있는 메인 스탯이 정해져 있으며, 다른 Cost의 메인 스탯을 장착할 수 없습니다.</size>", 10)));

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<size=27><b>3. 에코 전투 성장 시스템</b></size>\n\n" +
            "<size=21>각 에코는 고유 레벨을 가지고 있으며, 모두 독립적입니다.\n" +
            "초기 시작 시 에코는 1레벨로 시작하며, 최대 25레벨까지 레벨업이 가능합니다.\n" +
            "레벨은 퀘스트를 통해 경험치를 획득하여 올릴 수 있으며, 대부분의 행동으로 획득 가능합니다.\n\n" +
            "에코 레벨이 1 오를 때마다 메인 스탯이 선형 증가하며,\n" +
            "에코 레벨이 5의 배수일 때 마다 부옵션이 해금됩니다.\n" +
            "부옵션은 자동으로 부여되며, 한번 부여된 부옵션은 변경할 수 없습니다.</size>", 10)));

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<size=27><b>4. 에코 전투 액티브 스킬 목록 <color=#D2042D>(Cost 4)</color></b></size>\n\n" +
            "<size=19><color=#D2042D>광전사</color>: 2초간 자신의 최대 체력 10%만큼 데미지 보너스, 재사용 대기시간 60초\n" +
            "<color=#D2042D>카피바라</color>: 20초간 자신의 최대 체력 초당 2.5%만큼 회복, 재사용 대기시간 120초\n" +
            "<color=#D2042D>노움</color>: 5초간 데미지 감소 70%, 재사용 대기시간 60초\n" +
            "<color=#D2042D>살라만드라</color>: 30초간 공격에 화상 효과 부여, 재사용 대기시간 60초\n" +
            "<color=#D2042D>운디네</color>: 주변 15m 적에게 2초간 감속, 재사용 대기시간 40초\n" +
            "<color=#D2042D>실프</color>: 10초간 이동속도 증가 및 반투명/문 통과/발소리 제거, 재사용 대기시간 60초</size>", 10)));

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<size=27><b>4. 에코 전투 액티브 스킬 목록 <color=#FFBF00>(Cost 3)</color></b></size>\n\n" +
            "<size=19><color=#FFBF00>황금 돼지</color>: 사용 시 체력 1205 회복, 재사용 대기시간 60초\n" +
            "<color=#FFBF00>쁘띠 049</color>: SCP-049가 사용 시 다음 소생하는 049-2의 체력 50% 증가, 재사용 대기시간 30초\n" +
            "<color=#FFBF00>쁘띠 096</color>: SCP-096이 사용 시 즉시 분노, 주변 20m 대상 전체 목격자 포함, 받는 데미지 25% 감소, 재사용 대기시간 90초\n" +
            "<color=#FFBF00>쁘띠 106</color>: SCP-106이 사용 시 에너지 70% 회복, 재사용 대기시간 60초\n" +
            "<color=#FFBF00>쁘띠 173</color>: SCP-173이 사용 시 다음 순간이동 시간 1초로 감소, 재사용 대기시간 60초\n" +
            "<color=#FFBF00>쁘띠 939</color>: 15초간 스태미너 무제한, 재사용 대기시간 60초</size>", 10)));

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<size=27><b>5. 에코 전투 스탯 목록 (메인 스탯)</b></size>\n\n" +
            "<size=20>공격력, HP, 방어력은 Cost 구분 없이 모두 장착 가능합니다.\n\n" +
            "<color=#D2042D>Cost 4</color>: 크리티컬 확률, 크리티컬 데미지, 이동속도/점프력, 스태미너 소모 속도 감소\n" +
            "<color=#FFBF00>Cost 3</color>: SCP 대상 데미지, 인간 대상 데미지, 헤드샷 데미지, AHP 회복 및 최대치 증가, 크기 감소\n" +
            "Cost 1: 없음 (공격력, HP, 방어력 사용 가능)\n\n" +
            "부가 스탯 — <color=#D2042D>Cost 4</color>: 공격력 / <color=#FFBF00>Cost 3</color>: 치료 효과 보너스 / Cost 1: HP</size>", 10)));

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<size=27><b>5. 에코 전투 스탯 목록 (부옵션)</b></size>\n\n" +
            "<size=20>공격력%, 공격력, 방어력%, 방어력, HP%, HP,\n" +
            "크리티컬 확률, SCP 대상 데미지, 인간 대상 데미지,\n" +
            "이동속도, 점프력, 스태미너 소모 속도 감소, 헤드샷 데미지,\n" +
            "크기 감소, 치료 효과 보너스, 크리티컬 데미지</size>", 5)));

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<size=27><b>5. 에코 전투 스탯 목록 (기타)</b></size>\n\n" +
            "<size=20>공격력 관련 옵션의 경우 SCP-049와 SCP-106은 0.5배수 보정을 받으며,\n" +
            "HP 관련 옵션의 경우 SCP 진영은 12배 보정을 받습니다.(HP% 옵션은 제외)\n" +
            "SCP-173은 공격력 관련 옵션 영향을 받지 않습니다.</size>", 5)));

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<size=27><b>6. 에코 전투 전용무기 시스템</b></size>\n\n" +
            "<size=20>5개의 에코 이외에, 전용무기를 1개 장착할 수 있습니다.\n" +
            "전용무기는 에코와 독립적인 경험치와 능력치, 패시브 능력을 가집니다.\n" +
            "전용무기는 최대 90레벨까지 성장 가능합니다.\n\n" +
            "전용무기는 레벨 이외에 공진 수치라는 특별 강화 수치가 있으며,\n" +
            "특정 퀘스트 달성으로 올릴 수 있습니다.\n" +
            "특정 퀘스트는 에코 전투 상태창에서 확인 가능합니다.</size>", 10)));

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<size=27><b>7. 에코 전투 전용무기 목록</b></size>\n\n" +
            "<size=18><color=#FF3131>피안화</color>: HP 16% + (공진 수치 x 4%), 크리티컬 확률 5% 증가.\n" +
            "사망에 이르는 피해 시 (1.2초 x 공진 수치)간 무적·투명·이속 증가(최대 3회). 발동 시 최대 체력의 15% + (5% x 공진 수치) 회복.\n\n" +
            "<color=#5D3FD3>위조된 작은 별</color>: 공격력 11% + (공진 수치 x 2%) 증가. 화상 대상 공격 시 크리티컬 데미지 (12% x 공진 수치) 증가.\n" +
            "공격 적중 시 대상에게 불꽃 효과를 부여하며, 최대 6스택 중첩 가능. 2초 이내에 재적용 불가.\n" +
            "최대 스택 도달 시 불꽃을 폭파시키며, 주변 7m 적에게 50 + (10 x 공진 수치) + (최대 체력의 10% + 2% x 공진 수치) 의 피해를 입힘.\n\n" +
            "<color=#DC143C>쿠모키리</color>: 공격력 11% + (공진 수치 x 2%) 증가.\n" +
            "공격 적중 시 (1/3/6/10/15)%의 확률로 618.03 x (0.5 + 0.5 x 공진 수치)만큼 고정 피해.</size>", 10)));

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ShowInstruction(
            "<size=27><b>7. 에코 전투 전용무기 목록</b></size>\n\n" +
            "<size=18><color=#7DF9FF>서리</color>: 공격력 8% + (공진 수치 x 2%) 증가.\n" +
            "공격 적중 시 대상에게 서리 효과를 부여하며, 최대 10스택 중첩 가능, 0.5초 이내에 재적용 불가.\n" +
            "1스택 당 적의 이동 속도를 1 + (1 x 공진 수치)%만큼 감소시키며, 최대 스택 도달 시 대상을 2 + (0.6 x 공진 수치)초 만큼 속박.\n" +
            "속박된 대상은 5초 동안 서리 효과를 적용받지 않음.\n\n" +
            "<color=#87CEEB>밤하늘 연산 측정기</color>: HP 16% + (공진 수치 x 4%), 크리티컬 확률 5% 증가.\n" +
            "AHP(또는 HS)가 피해를 입을 경우, AHP(HS)의 순수 차감량의 (16% x 공진 수치)만큼 HP 회복.\n\n" +
            "<color=#FAFA33>스펙트럼 블래스터</color>: 공격력 8% + (공진 수치 x 2%) 증가.\n" +
            "적 공격 시 0.8초 간격으로 이동속도 (1 x 공진 수치)%만큼 증가. 최대 8스택 중첩, 5초 지속.\n" +
            "5초 내에 적 공격 시 효과 지속 시간 갱신.</size>", 10)));

        for (int i = 5; i > 0; i--)
        {
            ShowInstructionHint(
                "<align=center><size=23>이 모드에 대해 조금이라도 이해가 되었길 바라며,\n" +
                "모두 즐거운 에코 전투 되세요!</size>\n\n" +
                $"<size=28><b>{i}</b>초 뒤 라운드가 시작됩니다.</size></align>");
            yield return Timing.WaitForSeconds(1);
        }
    }

    IEnumerator<float> ShowInstruction(string content, float duration)
    {
        for (float elapsed = 0f; elapsed < duration; elapsed += 1f)
        {
            ShowInstructionHint(content);
            yield return Timing.WaitForSeconds(1f);
        }
    }

    void ShowInstructionHint(string content)
    {
        string hint = string.Format(InstructionHintFormat, content);
        foreach (var player in Player.List.Where(player => player.IsConnected && !player.IsNPC))
            player.ShowHint(hint, 1.2f);
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
        IEnumerable<Player> players = PlayerManager.List.Where(x => x.IsAlive && !x.IsNPC);

        switch (players.Count())
        {
            case 1:
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 20));
                break;
            case > 1:
                Timing.RunCoroutine(Tools.SetWinner(players.ToList(), 4));
                break;
        }
        
        foreach (var player in Player.List.ToList())
            ClearPlayerState(player, "라운드 종료");
    }

    void ClearPlayerState(Player player, string reason)
    {
        string playerId = player?.UserId ?? "null";

        RunCleanupStep(playerId, reason, nameof(EchoQuest.ClearPlayer),
            () => EchoQuest.ClearPlayer(player));
        RunCleanupStep(playerId, reason, nameof(EchoGrowth.ClearPending),
            () => EchoGrowth.ClearPending(player));
        RunCleanupStep(playerId, reason, nameof(ExclusiveWeaponGrowth.ClearPending),
            () => ExclusiveWeaponGrowth.ClearPending(player));
        RunCleanupStep(playerId, reason, nameof(ExclusiveWeaponCore.ClearAll),
            () => ExclusiveWeaponCore.ClearAll(player));
        RunCleanupStep(playerId, reason, nameof(EchoBattleCore.Reset),
            () => EchoBattleCore.Reset(player));
    }

    static void RunCleanupStep(string playerId, string reason, string step, System.Action action)
    {
        try
        {
            Log.Error($"[EchoBattle] {reason} 정리 시작: {step} (Player: {playerId})");
            action();
            Log.Error($"[EchoBattle] {reason} 정리 완료: {step} (Player: {playerId})");
        }
        catch (System.Exception exception)
        {
            Log.Error($"[EchoBattle] {reason} 정리 실패: {step} (Player: {playerId})\n{exception}");
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
