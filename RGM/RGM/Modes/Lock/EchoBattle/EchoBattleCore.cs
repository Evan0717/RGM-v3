using Exiled.API.Features;
using Exiled.API.Features.Roles;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using MEC;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;

namespace RGM.RGM.Modes.Lock.EchoBattle;

public static class EchoBattleCore
{
    // 화면 5x5 그리드에서 19번 위치 (0-index: row3 col3 → 우측 하단에서 약간 위)
    // Rank 힌트(-300, 80) 대비 우측 하단 쪽으로 배치
    const float HintX = 420f;
    const float HintY = 620f;

    public static void RegisterEchoes()
    {
        EchoInfo.Echoes.Clear();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var attr = type.GetCustomAttribute<EchoAttribute>();
            if (attr == null || !typeof(Echo).IsAssignableFrom(type))
                continue;

            EchoInfo.Echoes[attr.Type] = new EchoData
            {
                Type = type,
                Name = attr.Name,
                Description = attr.Description,
                Emoji = attr.Emoji,
                EchoType = attr.Type,
                Cost = attr.Cost,
                MainStatType = attr.MainStatType
            };
        }

        Log.Info($"[EchoBattle] Registered {EchoInfo.Echoes.Count} echoes.");
    }

    public static void AddEcho(Player player, EchoType type, int level, bool isMainSlot)
    {
        if (!EchoInfo.Echoes.TryGetValue(type, out var data))
        {
            Log.Error($"[EchoBattle] Echo {type} not found.");
            return;
        }

        Echo echo;
        try
        {
            echo = Activator.CreateInstance(data.Type) as Echo;
        }
        catch (Exception e)
        {
            Log.Error($"[EchoBattle] Failed to create {data.Name}: {e}");
            return;
        }

        if (echo == null)
            return;

        echo.Data = data;
        echo.Owner = player;
        echo.Level = EchoStats.Clamp(level, 1, EchoInfo.MaxLevel);
        echo.IsMainSlot = isMainSlot;
        echo.SubOptions = EchoStats.GenerateSubOptions(echo.Level, (player.UserId?.GetHashCode() ?? 0) ^ (int)type ^ level);

        if (!EchoInfo.PlayerEchoes.ContainsKey(player))
            EchoInfo.PlayerEchoes[player] = new();

        EchoInfo.PlayerEchoes[player].Add(echo);
        echo.OnEnabled();
    }

    public static void RemoveAllEchoes(Player player)
    {
        Timing.KillCoroutines($"EchoRegen_{player.UserId}");

        if (!EchoInfo.PlayerEchoes.TryGetValue(player, out var list))
            return;

        foreach (var echo in list)
            echo.OnDisabled();

        list.Clear();
        EchoInfo.PlayerStats.Remove(player);
    }

    public static void ApplyLoadout(Player player)
    {
        RemoveAllEchoes(player);

        if (!EchoInfo.PlayerLoadouts.TryGetValue(player, out var loadout))
        {
            EchoQuest.StopSurviveTracking(player);
            return;
        }

        if (loadout.MainSlot.HasValue)
            AddEcho(player, loadout.MainSlot.Value, loadout.GetLevel(loadout.MainSlot.Value), true);

        foreach (var slot in loadout.SubSlots)
        {
            if (slot.HasValue)
                AddEcho(player, slot.Value, loadout.GetLevel(slot.Value), false);
        }

        if (!EchoInfo.PlayerEchoes.TryGetValue(player, out var echoes) || echoes.Count == 0)
        {
            EchoQuest.StopSurviveTracking(player);
            return;
        }

        var snapshot = EchoStats.BuildSnapshot(player, echoes);
        EchoInfo.PlayerStats[player] = snapshot;
        EchoStats.ApplyPassiveEffects(player, snapshot);

        EchoQuest.StartSurviveTracking(player);
    }

    public static void Reset(Player player)
    {
        EchoQuest.StopSurviveTracking(player);
        RemoveAllEchoes(player);
    }

    public static IEnumerator<float> HintDisplay(Player owner)
    {
        Hint hint = new() { Text = "" };

        while (true)
        {
            Player target = owner;
            bool shown = false;

            try
            {
                if (owner.Role is SpectatorRole spectator && spectator.SpectatedPlayer != null)
                    target = spectator.SpectatedPlayer;

                if (target != null && target.IsAlive)
                {
                    string text = BuildHintText(target);
                    if (!string.IsNullOrEmpty(text))
                    {
                        hint = new Hint
                        {
                            Text = $"<size=14>{text}</size>",
                            Id = "EchoBattleHint",
                            XCoordinate = HintX,
                            YCoordinate = HintY,
                            Alignment = HintAlignment.Right
                        };

                        owner.AddCustomHint(hint);
                        shown = true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug($"[EchoBattle] Hint error: {e.Message}");
            }

            if (shown)
            {
                yield return Timing.WaitForOneFrame;
                owner.RemoveHint(hint);
            }
            else
            {
                yield return Timing.WaitForOneFrame;
            }
        }
    }

    static string BuildHintText(Player player)
    {
        var lines = new List<string>();

        if (!EchoInfo.PlayerEchoes.TryGetValue(player, out var echoes) || echoes.Count == 0)
        {
            lines.Add("<color=#88aaff>[ESC] → Settings → Server-specific</color>");
            lines.Add("Echo를 장착하세요.");
            return string.Join("\n", lines);
        }

        EchoInfo.PlayerLoadouts.TryGetValue(player, out var loadout);

        var main = echoes.FirstOrDefault(x => x.IsMainSlot);
        if (main is EchoActiveAbility active)
        {
            string status;
            if (active.RemainingDuration > 0.1f)
                status = $"<color=#7CFC00>지속 {active.RemainingDuration:0}초</color>";
            else if (active.IsOnCooldown)
                status = $"<color=#ff6666>쿨 {active.RemainingCooldown:0}초</color>";
            else
                status = "<color=#7CFC00>사용 가능</color>";

            lines.Add($"<b>{main.Data.GetFormattedName()}</b> Lv.{main.Level}");
            if (loadout != null)
                lines.Add($"XP {EchoGrowth.FormatProgress(loadout, main.Data.EchoType)}");
            lines.Add($"[ALT] {status}");
        }
        else if (main != null)
        {
            lines.Add($"<b>{main.Data.GetFormattedName()}</b> Lv.{main.Level}");
            if (loadout != null)
                lines.Add($"XP {EchoGrowth.FormatProgress(loadout, main.Data.EchoType)}");
        }

        // 장착 Echo 레벨 요약
        if (loadout != null)
        {
            var equippedLines = new List<string>();
            foreach (var echo in echoes)
            {
                if (echo?.Data == null)
                    continue;
                equippedLines.Add($"{echo.Data.Emoji}{echo.Data.Name} Lv.{echo.Level} ({EchoGrowth.FormatProgress(loadout, echo.Data.EchoType)})");
            }

            if (equippedLines.Count > 0)
            {
                lines.Add("<color=#aaaaaa>── Echo ──</color>");
                lines.AddRange(equippedLines);
            }
        }

        if (EchoInfo.PlayerStats.TryGetValue(player, out var stats))
        {
            lines.Add("<color=#aaaaaa>── 강화 합산 ──</color>");
            foreach (var pair in stats.GetAggregatedDisplay())
                lines.Add($"{pair.Key}: <b>{pair.Value:0.#}</b>");
        }

        return string.Join("\n", lines);
    }
}
