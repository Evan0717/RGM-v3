using Exiled.API.Features;
using RGM.API.Features;
using System;
using System.Collections.Generic;

namespace RGM.RGM.Modes.Lock.EchoBattle;

/// <summary>
/// Echo 성장 XP 공식.
/// 1→2 기준: Cost4=150, Cost3=115, Cost1=60
/// 이후: y = ceil(1.06x + 10)
/// </summary>
public static class EchoGrowth
{
    public const float LevelExpMultiplier = 1.06f;
    public const float LevelExpAdd = 10f;

    public static int GetBaseExp(EchoCost cost)
    {
        return cost switch
        {
            EchoCost.Cost4 => 150,
            EchoCost.Cost3 => 115,
            EchoCost.Cost1 => 60,
            _ => 60
        };
    }

    /// <summary>
    /// currentLevel → currentLevel+1 에 필요한 경험치.
    /// </summary>
    public static int GetRequiredExp(EchoCost cost, int currentLevel)
    {
        currentLevel = EchoStats.Clamp(currentLevel, 1, EchoInfo.MaxLevel);
        if (currentLevel >= EchoInfo.MaxLevel)
            return 0;

        int exp = GetBaseExp(cost);
        // level 1→2 는 base, 그 다음부터 공식 적용
        for (int level = 1; level < currentLevel; level++)
            exp = NextExp(exp);

        return exp;
    }

    public static int NextExp(int previousRequired)
    {
        return (int)Math.Ceiling(LevelExpMultiplier * previousRequired + LevelExpAdd);
    }

    /// <summary>
    /// 장착 중인 모든 Echo에 XP를 지급하고, 레벨업 시 로드아웃/인스턴스를 갱신합니다.
    /// </summary>
    public static void GrantExpToEquipped(Player player, int amount, string reason = null)
    {
        if (player == null || amount <= 0)
            return;

        if (!EchoInfo.PlayerLoadouts.TryGetValue(player, out var loadout))
            return;

        var leveledUp = new List<(EchoType Type, int NewLevel)>();

        foreach (var type in loadout.GetEquipped())
        {
            if (AddExp(player, loadout, type, amount, out int newLevel))
                leveledUp.Add((type, newLevel));
        }

        foreach (var (type, newLevel) in leveledUp)
        {
            var data = type.GetData();
            string name = data?.Name ?? type.ToString();
            player.AddBroadcast(3, $"<size=25><color=#7CFC00>{name}</color> → Lv.<b>{newLevel}</b></size>");
        }

        if (leveledUp.Count > 0 && EchoInfo.PlayerEchoes.ContainsKey(player))
        {
            // Hurting 이벤트 중 즉시 재적용하면 핸들러 충돌 가능 → 다음 프레임에 적용
            MEC.Timing.CallDelayed(MEC.Timing.WaitForOneFrame, () =>
            {
                if (player != null && player.IsAlive)
                    EchoBattleCore.ApplyLoadout(player);
            });
        }
    }

    /// <summary>
    /// 특정 Echo에 XP 추가. 레벨업 발생 시 true.
    /// </summary>
    public static bool AddExp(Player player, EchoLoadout loadout, EchoType type, int amount, out int newLevel)
    {
        newLevel = loadout.GetLevel(type);
        if (amount <= 0 || newLevel >= EchoInfo.MaxLevel)
            return false;

        var data = type.GetData();
        if (data == null)
            return false;

        if (!loadout.Experience.ContainsKey(type))
            loadout.Experience[type] = 0;

        loadout.Experience[type] += amount;
        bool leveled = false;

        while (newLevel < EchoInfo.MaxLevel)
        {
            int required = GetRequiredExp(data.Cost, newLevel);
            if (required <= 0 || loadout.Experience[type] < required)
                break;

            loadout.Experience[type] -= required;
            newLevel++;
            loadout.Levels[type] = newLevel;
            leveled = true;
        }

        if (newLevel >= EchoInfo.MaxLevel)
            loadout.Experience[type] = 0;

        return leveled;
    }

    public static int GetCurrentExp(EchoLoadout loadout, EchoType type)
    {
        return loadout.Experience.TryGetValue(type, out int exp) ? exp : 0;
    }

    public static string FormatProgress(EchoLoadout loadout, EchoType type)
    {
        int level = loadout.GetLevel(type);
        if (level >= EchoInfo.MaxLevel)
            return "MAX";

        var data = type.GetData();
        if (data == null)
            return "?";

        int current = GetCurrentExp(loadout, type);
        int required = GetRequiredExp(data.Cost, level);
        return $"{current}/{required}";
    }
}
