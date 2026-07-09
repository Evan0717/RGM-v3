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

Quest (반복)
• 30초 생존 → 130 XP
• 적에게 50 데미지 → 100 XP
• 25 데미지 받기 → 100 XP

[ESC] -> [Settings] -> [Server-specific]
""";
    public override string Color => "023e8a";
    public override string Author => "Denia's First Project";

    /// <summary>테스트용: true면 RoundLock + AFK 추방 방지를 켭니다.</summary>
    const bool SoloTestMode = true;

    CoroutineHandle _onModeStarted;
    readonly Dictionary<Player, CoroutineHandle> _hintHandles = new();
    readonly Dictionary<Player, CoroutineHandle> _applyHandles = new();

    public override void OnEnabled()
    {
        if (SoloTestMode)
            Round.IsLocked = true;

        EchoBattleCore.RegisterEchoes();

        Exiled.Events.Handlers.Player.Verified += OnVerified;
        Exiled.Events.Handlers.Player.Hurting += EchoStats.OnHurting;
        if (SoloTestMode)
            Exiled.Events.Handlers.Player.Kicking += OnKicking;
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

        EchoQuest.Register();
        EchoSetting.Init();
        ServerSpecificSettingsSync.ServerOnSettingValueReceived += EchoSetting.OnSSInput;

        _onModeStarted = Timing.RunCoroutine(OnModeStarted());
    }

    public override void OnDisabled()
    {
        if (SoloTestMode)
            Round.IsLocked = false;

        Exiled.Events.Handlers.Player.Verified -= OnVerified;
        Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
        Exiled.Events.Handlers.Player.Hurting -= EchoStats.OnHurting;
        if (SoloTestMode)
            Exiled.Events.Handlers.Player.Kicking -= OnKicking;
        Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

        ServerSpecificSettingsSync.ServerOnSettingValueReceived -= EchoSetting.OnSSInput;
        EchoQuest.Unregister();

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
    }

    IEnumerator<float> OnModeStarted()
    {
        foreach (var p in Player.List)
            Verified(p);

        for (int i = 0; i < EchoInfo.ApplyDelaySeconds; i++)
        {
            foreach (var player in Player.List)
            {
                player.AddBroadcast(1,
                    $"<size=30>Echo 적용까지 <size=50><b>{EchoInfo.ApplyDelaySeconds - i}</b></size>초</size>\n" +
                    $"<size=20>[ESC] -> [Settings] -> [Server-specific]ㅣEcho를 미리 장착해두세요.</size>");

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
            EchoBattleCore.ApplyLoadout(player);
        }
    }

    void OnVerified(VerifiedEventArgs ev) => Verified(ev.Player);

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
        EchoBattleCore.Reset(ev.Player);

        if (!ev.NewRole.IsAlive())
            return;

        _applyHandles[ev.Player] = Timing.RunCoroutine(ApplyAfterDelay(ev.Player));
    }

    IEnumerator<float> ApplyAfterDelay(Player player)
    {
        for (int i = 0; i < EchoInfo.ApplyDelaySeconds; i++)
        {
            if (player == null || !player.IsAlive)
                yield break;

            player.AddBroadcast(1,
                $"<size=30>Echo 적용까지 <size=50><b>{EchoInfo.ApplyDelaySeconds - i}</b></size>초</size>\n" +
                $"<size=20>[ESC] -> [Settings] -> [Server-specific]</size>");

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
            EchoBattleCore.Reset(player);
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
