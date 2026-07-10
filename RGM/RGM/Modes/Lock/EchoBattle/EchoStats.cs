using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.Modes;

/// <summary>
/// Echo Main/Sub 스탯 및 부가 옵션 수치 계산/적용.
/// 레벨 1~25 선형 증가. 부가 옵션은 레벨 5의 배수마다 1개 해금.
/// </summary>
public static class EchoStats
{
    static readonly RoleTypeId[] AttackFlatIgnoredRoles =
    {
        RoleTypeId.Scp173,
        RoleTypeId.Scp049,
        RoleTypeId.Scp079
    };

    static readonly DamageType[] DefenseFlatIgnoredDamageTypes =
    {
        DamageType.Warhead,
        DamageType.Crushed,
        DamageType.PocketDimension,
        DamageType.Falldown,
        DamageType.Scp106
    };

    static readonly float[] SubOptionGradeWeights = { 250, 220, 190, 150, 110, 80 };

    static readonly Dictionary<EchoSubOptionType, float[]> SubOptionValues = new()
    {
        { EchoSubOptionType.AttackPercent, [6.5f, 7.4f, 8.3f, 9.2f, 10.1f, 11.0f] },
        { EchoSubOptionType.AttackFlat, [2f, 4f, 6f, 8f, 10f, 12f] },
        { EchoSubOptionType.DefensePercent, [2.0f, 2.5f, 3.0f, 3.5f, 4.0f, 4.5f] },
        { EchoSubOptionType.DefenseFlat, [3.0f, 3.4f, 3.8f, 4.2f, 4.6f, 5.0f] },
        { EchoSubOptionType.HpPercent, [8.1f, 9.2f, 10.3f, 11.4f, 12.5f, 13.6f] },
        { EchoSubOptionType.HpFlat, [20f, 26f, 32f, 38f, 44f, 50f] },
        { EchoSubOptionType.CriticalChance, [3.3f, 3.7f, 4.1f, 4.5f, 4.9f, 5.3f] },
        { EchoSubOptionType.ScpDamagePercent, [8.3f, 9.6f, 10.9f, 12.2f, 13.5f, 14.8f] },
        { EchoSubOptionType.HumanDamagePercent, [8.3f, 9.6f, 10.9f, 12.2f, 13.5f, 14.8f] },
        { EchoSubOptionType.MoveSpeed, [5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f] },
        { EchoSubOptionType.JumpPower, [2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f] },
        { EchoSubOptionType.StaminaDrainReduction, [8.3f, 9.2f, 10.1f, 11.0f, 11.9f, 12.8f] },
        { EchoSubOptionType.HeadshotDamage, [20.8f, 22.5f, 24.2f, 25.9f, 27.6f, 29.3f] },
    };

    public static float LerpStat(float min, float max, int level)
    {
        level = Clamp(level, 1, EchoInfo.MaxLevel);
        float t = (level - 1) / (float)(EchoInfo.MaxLevel - 1);
        return min + (max - min) * t;
    }

    public static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static float GetMainStatValue(EchoCost cost, EchoMainStatType type, int level)
    {
        return (cost, type) switch
        {
            (EchoCost.Cost4, EchoMainStatType.AttackPercent) => LerpStat(6.6f, 33.0f, level),
            (EchoCost.Cost4, EchoMainStatType.HpPercent) => LerpStat(12.5f, 62.5f, level),
            (EchoCost.Cost4, EchoMainStatType.Defense) => LerpStat(4.0f, 20.0f, level),
            (EchoCost.Cost4, EchoMainStatType.CriticalChance) => LerpStat(4.4f, 22.0f, level),
            (EchoCost.Cost4, EchoMainStatType.MoveSpeedAndJump) => LerpStat(10.0f, 60.0f, level),

            (EchoCost.Cost3, EchoMainStatType.AttackPercent) => LerpStat(5.2f, 26.0f, level),
            (EchoCost.Cost3, EchoMainStatType.HpPercent) => LerpStat(8.6f, 43.0f, level),
            (EchoCost.Cost3, EchoMainStatType.Defense) => LerpStat(2.0f, 10.0f, level),
            (EchoCost.Cost3, EchoMainStatType.ScpDamagePercent) => LerpStat(8.5f, 42.5f, level),
            (EchoCost.Cost3, EchoMainStatType.HumanDamagePercent) => LerpStat(8.5f, 42.5f, level),
            (EchoCost.Cost3, EchoMainStatType.StaminaDrainReduction) => LerpStat(4.4f, 22.0f, level),
            (EchoCost.Cost3, EchoMainStatType.HeadshotDamage) => LerpStat(17.0f, 85.0f, level),
            (EchoCost.Cost3, EchoMainStatType.AhpRegenAndMax) => LerpStat(1.6f, 8.0f, level),

            (EchoCost.Cost1, EchoMainStatType.AttackPercent) => LerpStat(3.2f, 16.0f, level),
            (EchoCost.Cost1, EchoMainStatType.HpPercent) => LerpStat(5.4f, 27.0f, level),
            (EchoCost.Cost1, EchoMainStatType.Defense) => LerpStat(1.0f, 5.0f, level),

            _ => 0f
        };
    }

    /// <summary>Cost별로 선택 가능한 메인 스탯 목록.</summary>
    public static IReadOnlyList<EchoMainStatType> GetAvailableMainStats(EchoCost cost)
    {
        return cost switch
        {
            EchoCost.Cost4 => Cost4MainStats,
            EchoCost.Cost3 => Cost3MainStats,
            EchoCost.Cost1 => Cost1MainStats,
            _ => Array.Empty<EchoMainStatType>()
        };
    }

    static readonly EchoMainStatType[] Cost4MainStats =
    {
        EchoMainStatType.AttackPercent,
        EchoMainStatType.HpPercent,
        EchoMainStatType.Defense,
        EchoMainStatType.CriticalChance,
        EchoMainStatType.MoveSpeedAndJump,
    };

    static readonly EchoMainStatType[] Cost3MainStats =
    {
        EchoMainStatType.AttackPercent,
        EchoMainStatType.HpPercent,
        EchoMainStatType.Defense,
        EchoMainStatType.ScpDamagePercent,
        EchoMainStatType.HumanDamagePercent,
        EchoMainStatType.StaminaDrainReduction,
        EchoMainStatType.HeadshotDamage,
        EchoMainStatType.AhpRegenAndMax,
    };

    static readonly EchoMainStatType[] Cost1MainStats =
    {
        EchoMainStatType.AttackPercent,
        EchoMainStatType.HpPercent,
        EchoMainStatType.Defense,
    };

    public static bool IsMainStatAvailable(EchoCost cost, EchoMainStatType type)
    {
        if (type == EchoMainStatType.None)
            return false;

        foreach (var available in GetAvailableMainStats(cost))
        {
            if (available == type)
                return true;
        }

        return false;
    }

    /// <summary>Server-Specific 드롭다운용: 모든 Cost에서 등장하는 메인 스탯 합집합.</summary>
    public static IReadOnlyList<EchoMainStatType> GetAllSelectableMainStats()
    {
        return AllSelectableMainStats;
    }

    static readonly EchoMainStatType[] AllSelectableMainStats =
    {
        EchoMainStatType.AttackPercent,
        EchoMainStatType.HpPercent,
        EchoMainStatType.Defense,
        EchoMainStatType.ScpDamagePercent,
        EchoMainStatType.HumanDamagePercent,
        EchoMainStatType.CriticalChance,
        EchoMainStatType.MoveSpeedAndJump,
        EchoMainStatType.StaminaDrainReduction,
        EchoMainStatType.HeadshotDamage,
        EchoMainStatType.AhpRegenAndMax,
    };

    public static float GetSubStatValue(EchoCost cost, int level)
    {
        return cost switch
        {
            EchoCost.Cost4 => LerpStat(3f, 30f, level),
            EchoCost.Cost3 => LerpStat(1f, 12f, level),
            EchoCost.Cost1 => LerpStat(12f, 150f, level),
            _ => 0f
        };
    }

    public static EchoSubStatType GetSubStatType(EchoCost cost)
    {
        return cost == EchoCost.Cost1 ? EchoSubStatType.HpFlat : EchoSubStatType.AttackFlat;
    }

    public static int GetUnlockedSubOptionCount(int level)
    {
        return EchoStats.Clamp(level / 5, 0, 5);
    }

    static List<EchoSubOptionType> GetAllSubOptionTypes()
    {
        return Enum.GetValues(typeof(EchoSubOptionType)).Cast<EchoSubOptionType>()
            .Where(x => x != EchoSubOptionType.None)
            .ToList();
    }

    /// <summary>
    /// 로드아웃에 저장된 부가 옵션을 유지한 채, 해금 개수만큼만 새로 굴립니다.
    /// 단일 Echo 안에서는 같은 종류의 부가 옵션이 중복되지 않습니다.
    /// </summary>
    public static List<EchoSubOptionInstance> EnsureSubOptions(EchoLoadout loadout, EchoType type, int level, int seedBase)
    {
        if (!loadout.SubOptions.TryGetValue(type, out var existing) || existing == null)
        {
            existing = new List<EchoSubOptionInstance>();
            loadout.SubOptions[type] = existing;
        }

        // 이미 저장된 중복 타입은 첫 번째만 유지
        DeduplicateSubOptions(existing);

        int targetCount = GetUnlockedSubOptionCount(level);

        while (existing.Count > targetCount)
            existing.RemoveAt(existing.Count - 1);

        var used = new HashSet<EchoSubOptionType>(existing.Select(x => x.Type));
        var pool = GetAllSubOptionTypes().Where(x => !used.Contains(x)).ToList();

        while (existing.Count < targetCount && pool.Count > 0)
        {
            int slotIndex = existing.Count;
            var rng = new System.Random(seedBase ^ (slotIndex * 397) ^ 0x5F3759DF);
            int pick = rng.Next(pool.Count);
            var optionType = pool[pick];
            pool.RemoveAt(pick);

            int grade = RollGrade(rng);
            float value = SubOptionValues[optionType][grade - 1];
            existing.Add(new EchoSubOptionInstance
            {
                Type = optionType,
                Grade = grade,
                Value = value
            });
        }

        return existing.Select(CloneSubOption).ToList();
    }

    /// <summary>같은 Echo 내 중복 타입은 먼저 해금된(앞쪽) 옵션만 남깁니다.</summary>
    static void DeduplicateSubOptions(List<EchoSubOptionInstance> options)
    {
        var seen = new HashSet<EchoSubOptionType>();
        for (int i = 0; i < options.Count;)
        {
            if (!seen.Add(options[i].Type))
                options.RemoveAt(i);
            else
                i++;
        }
    }

    public static EchoSubOptionInstance CloneSubOption(EchoSubOptionInstance option)
    {
        return new EchoSubOptionInstance
        {
            Type = option.Type,
            Grade = option.Grade,
            Value = option.Value
        };
    }

    public static List<EchoSubOptionInstance> GenerateSubOptions(int level, int? seed = null)
    {
        var result = new List<EchoSubOptionInstance>();
        int count = GetUnlockedSubOptionCount(level);
        var rng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        var pool = GetAllSubOptionTypes();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int pick = rng.Next(pool.Count);
            var optionType = pool[pick];
            pool.RemoveAt(pick);

            int grade = RollGrade(rng);
            float value = SubOptionValues[optionType][grade - 1];
            result.Add(new EchoSubOptionInstance
            {
                Type = optionType,
                Grade = grade,
                Value = value
            });
        }

        return result;
    }

    static int RollGrade(System.Random rng)
    {
        float total = SubOptionGradeWeights.Sum();
        float roll = (float)(rng.NextDouble() * total);
        float cumulative = 0f;

        for (int i = 0; i < SubOptionGradeWeights.Length; i++)
        {
            cumulative += SubOptionGradeWeights[i];
            if (roll <= cumulative)
                return i + 1;
        }

        return 1;
    }

    public static EchoStatSnapshot BuildSnapshot(Player player, List<Echo> echoes)
    {
        var snapshot = new EchoStatSnapshot();

        foreach (var echo in echoes)
        {
            if (echo?.Data == null)
                continue;

            int level = echo.Level;
            var cost = echo.Data.Cost;

            // Cost에 등록된 메인 스탯만 적용. 미등록/잔존 값은 무시.
            var mainType = echo.SelectedMainStat;
            if (!IsMainStatAvailable(cost, mainType))
                mainType = IsMainStatAvailable(cost, echo.Data.MainStatType)
                    ? echo.Data.MainStatType
                    : EchoMainStatType.None;

            if (mainType != EchoMainStatType.None)
            {
                float mainValue = GetMainStatValue(cost, mainType, level);
                if (mainValue > 0f || mainType == EchoMainStatType.AhpRegenAndMax)
                    ApplyMainStat(snapshot, mainType, mainValue, cost, level);
            }

            ApplySubStat(snapshot, cost, level, player);

            foreach (var option in echo.SubOptions)
                ApplySubOption(snapshot, option, player);
        }

        return snapshot;
    }

    static void ApplyMainStat(EchoStatSnapshot snapshot, EchoMainStatType type, float value, EchoCost cost, int level)
    {
        // 이중 방어: Cost에 없는 타입은 절대 반영하지 않음
        if (!IsMainStatAvailable(cost, type))
            return;

        switch (type)
        {
            case EchoMainStatType.AttackPercent:
                snapshot.AttackPercent += value;
                break;
            case EchoMainStatType.HpPercent:
                snapshot.HpPercent += value;
                break;
            case EchoMainStatType.Defense:
                snapshot.DefensePercent += value;
                break;
            case EchoMainStatType.ScpDamagePercent:
                snapshot.ScpDamagePercent += value;
                break;
            case EchoMainStatType.HumanDamagePercent:
                snapshot.HumanDamagePercent += value;
                break;
            case EchoMainStatType.CriticalChance:
                snapshot.CriticalChance += value;
                break;
            case EchoMainStatType.MoveSpeedAndJump:
                snapshot.MoveSpeed += value;
                snapshot.JumpPower += Mathf.Floor(value * 0.5f);
                break;
            case EchoMainStatType.StaminaDrainReduction:
                snapshot.StaminaDrainReduction += value;
                break;
            case EchoMainStatType.HeadshotDamage:
                snapshot.HeadshotDamage += value;
                break;
            case EchoMainStatType.AhpRegenAndMax:
                // Cost3 전용. regen value + max 테이블
                snapshot.AhpRegen += value;
                snapshot.AhpMax += LerpStat(20f, 150f, level);
                snapshot.HsRegen += LerpStat(2f, 15f, level);
                snapshot.HsMax += LerpStat(120f, 1000f, level);
                break;
        }
    }

    static void ApplySubStat(EchoStatSnapshot snapshot, EchoCost cost, int level, Player player)
    {
        float value = GetSubStatValue(cost, level);

        if (cost == EchoCost.Cost1)
        {
            float hp = value;
            if (player.IsScp)
                hp *= 8f;
            snapshot.HpFlat += hp;
        }
        else
        {
            if (AttackFlatIgnoredRoles.Contains(player.Role.Type))
                return;

            float attack = value;
            if (player.Role.Type == RoleTypeId.Scp939)
                attack *= 2f;
            snapshot.AttackFlat += attack;
        }
    }

    static void ApplySubOption(EchoStatSnapshot snapshot, EchoSubOptionInstance option, Player player)
    {
        switch (option.Type)
        {
            case EchoSubOptionType.AttackPercent:
                snapshot.AttackPercent += option.Value;
                break;
            case EchoSubOptionType.AttackFlat:
                if (AttackFlatIgnoredRoles.Contains(player.Role.Type))
                    break;
                snapshot.AttackFlat += player.Role.Type == RoleTypeId.Scp939 ? option.Value * 2f : option.Value;
                break;
            case EchoSubOptionType.DefensePercent:
                snapshot.DefensePercent += option.Value;
                break;
            case EchoSubOptionType.DefenseFlat:
                snapshot.DefenseFlat += option.Value;
                break;
            case EchoSubOptionType.HpPercent:
                snapshot.HpPercent += option.Value;
                break;
            case EchoSubOptionType.HpFlat:
                snapshot.HpFlat += player.IsScp ? option.Value * 8f : option.Value;
                break;
            case EchoSubOptionType.CriticalChance:
                snapshot.CriticalChance += option.Value;
                break;
            case EchoSubOptionType.ScpDamagePercent:
                snapshot.ScpDamagePercent += option.Value;
                break;
            case EchoSubOptionType.HumanDamagePercent:
                snapshot.HumanDamagePercent += option.Value;
                break;
            case EchoSubOptionType.MoveSpeed:
                snapshot.MoveSpeed += option.Value;
                break;
            case EchoSubOptionType.JumpPower:
                snapshot.JumpPower += option.Value;
                break;
            case EchoSubOptionType.StaminaDrainReduction:
                snapshot.StaminaDrainReduction += option.Value;
                break;
            case EchoSubOptionType.HeadshotDamage:
                snapshot.HeadshotDamage += option.Value;
                break;
        }
    }

    public static void ClearPassiveEffects(Player player)
    {
        if (player == null)
            return;

        Timing.KillCoroutines($"EchoRegen_{player.UserId}");
        Timing.KillCoroutines($"EchoStamina_{player.UserId}");

        if (!EchoInfo.PlayerPassiveEffects.TryGetValue(player, out var prev))
            return;

        if (prev.DefenseReduction > 0)
            player.RemoveEffect(EffectType.DamageReduction, prev.DefenseReduction);
        if (prev.MovementBoost > 0)
            player.RemoveEffect(EffectType.MovementBoost, prev.MovementBoost);
        if (prev.Lightweight > 0)
            player.RemoveEffect(EffectType.Lightweight, prev.Lightweight);
        if (prev.StaminaDrainToggled)
            player.IsUsingStamina = true;

        if (prev.EchoAhpKillCode.HasValue)
            KillEchoAhpProcess(player, prev.EchoAhpKillCode.Value);

        EchoInfo.PlayerPassiveEffects.Remove(player);
    }

    static void KillEchoAhpProcess(Player player, int killCode)
    {
        if (player?.ReferenceHub?.playerStats == null)
            return;

        var ahpStat = player.ReferenceHub.playerStats.GetModule<AhpStat>();
        ahpStat?.ServerKillProcess(killCode);
    }

    static bool TryGetEchoAhpProcess(Player player, int? killCode, out AhpProcess process)
    {
        process = null;
        if (!killCode.HasValue || player?.ReferenceHub?.playerStats == null)
            return false;

        var ahpStat = player.ReferenceHub.playerStats.GetModule<AhpStat>();
        return ahpStat != null && ahpStat.ServerTryGetProcess(killCode.Value, out process);
    }

    /// <summary>
    /// ClearPassiveEffects / RemoveAllEchoes 호출 전에 스탯 AHP 현재량을 읽습니다.
    /// </summary>
    public static bool TryPeekEchoAhpAmount(Player player, out float amount)
    {
        amount = 0f;
        if (player == null
            || !EchoInfo.PlayerPassiveEffects.TryGetValue(player, out var state)
            || !TryGetEchoAhpProcess(player, state.EchoAhpKillCode, out var process))
            return false;

        amount = process.CurrentAmount;
        return true;
    }

    public static void ApplyPassiveEffects(Player player, EchoStatSnapshot snapshot, float? preservedEchoAhp = null)
    {
        // 호출 측에서 넘기지 않은 경우(직접 재적용)에만 여기서 보존 시도
        bool hadEchoAhp = preservedEchoAhp.HasValue;
        float echoAhpAmount = preservedEchoAhp ?? 0f;
        if (!hadEchoAhp && TryPeekEchoAhpAmount(player, out float peeked))
        {
            hadEchoAhp = true;
            echoAhpAmount = peeked;
        }

        ClearPassiveEffects(player);

        // HP: 역할 기본 MaxHealth 기준으로만 계산 (레벨업 재적용 시 복리 방지)
        if (!EchoInfo.PlayerBaseMaxHealth.TryGetValue(player, out float baseMax) || baseMax <= 0f)
        {
            baseMax = player.MaxHealth;
            EchoInfo.PlayerBaseMaxHealth[player] = baseMax;
        }

        float newMax = baseMax * (1f + snapshot.HpPercent / 100f) + snapshot.HpFlat;
        float ratio = player.Health / Math.Max(1f, player.MaxHealth);
        player.MaxHealth = newMax;
        player.Health = Mathf.Clamp(newMax * ratio, 1f, newMax);

        var effectState = new EchoPassiveEffectState();

        // 방어력%: DamageReduction (intensity ≈ percent * 2, Rank 방어 참고)
        // 스냅샷에 이미 모든 Echo 합산값이 들어 있으므로 한 번만 적용
        if (snapshot.DefensePercent > 0)
        {
            effectState.DefenseReduction = (byte)Mathf.Clamp(Mathf.RoundToInt(snapshot.DefensePercent * 2f), 1, 255);
            player.AddEffect(EffectType.DamageReduction, effectState.DefenseReduction);
        }

        // 이동속도 / 점프력: 스냅샷 합산값을 이펙트로 1회 적용
        if (snapshot.MoveSpeed > 0)
        {
            effectState.MovementBoost = (byte)Mathf.Clamp(Mathf.RoundToInt(snapshot.MoveSpeed), 1, 255);
            player.AddEffect(EffectType.MovementBoost, effectState.MovementBoost);
        }

        if (snapshot.JumpPower > 0)
        {
            effectState.Lightweight = (byte)Mathf.Clamp(Mathf.RoundToInt(snapshot.JumpPower), 1, 255);
            player.AddEffect(EffectType.Lightweight, effectState.Lightweight);
        }

        if (snapshot.StaminaDrainReduction > 0)
        {
            effectState.StaminaDrainToggled = true;
            Timing.RunCoroutine(StaminaDrainReductionRoutine(player, snapshot.StaminaDrainReduction), $"EchoStamina_{player.UserId}");
        }

        EchoInfo.PlayerPassiveEffects[player] = effectState;

        // AHP / HS: 역할 기본값 + 스냅샷 증가량 (재적용 시 Max/현재값 복리 방지)
        if (player.IsScp)
        {
            // Role/SCP별 기본 HS를 최초 1회 저장한 뒤, 증가량만 더함
            if (!EchoInfo.PlayerBaseMaxHs.TryGetValue(player, out float baseHs))
            {
                baseHs = player.MaxHumeShield;
                EchoInfo.PlayerBaseMaxHs[player] = baseHs;
            }

            float targetHsMax = baseHs + snapshot.HsMax;
            player.MaxHumeShield = targetHsMax;
            player.HumeShield = Math.Min(player.HumeShield, player.MaxHumeShield);
        }
        else if (snapshot.AhpMax > 0)
        {
            // 스탯 AHP만 decay=0 별도 프로세스로 지급. 아드레날린 등 다른 AHP는 그대로 decay.
            float amount = hadEchoAhp
                ? Mathf.Min(echoAhpAmount, snapshot.AhpMax)
                : snapshot.AhpMax;

            var ahpStat = player.ReferenceHub.playerStats.GetModule<AhpStat>();
            var process = ahpStat.ServerAddProcess(amount, snapshot.AhpMax, 0f, 0.7f, 0f, true);
            effectState.EchoAhpKillCode = process.KillCode;
        }

        if (snapshot.AhpRegen > 0 || snapshot.HsRegen > 0)
            Timing.RunCoroutine(RegenRoutine(player, snapshot), $"EchoRegen_{player.UserId}");
    }

    static IEnumerator<float> StaminaDrainReductionRoutine(Player player, float reductionPercent)
    {
        const float cycleSeconds = 1f;
        float noDrainSeconds = cycleSeconds * Mathf.Clamp(reductionPercent, 0f, 100f) / 100f;
        float drainSeconds = cycleSeconds - noDrainSeconds;

        while (player != null && player.IsAlive && EchoInfo.PlayerStats.ContainsKey(player))
        {
            if (noDrainSeconds > 0f)
            {
                player.IsUsingStamina = false;
                yield return Timing.WaitForSeconds(noDrainSeconds);
            }

            if (drainSeconds > 0f)
            {
                player.IsUsingStamina = true;
                yield return Timing.WaitForSeconds(drainSeconds);
            }
        }

        if (player != null)
            player.IsUsingStamina = true;
    }

    static IEnumerator<float> RegenRoutine(Player player, EchoStatSnapshot snapshot)
    {
        while (player != null && player.IsAlive && EchoInfo.PlayerStats.ContainsKey(player))
        {
            if (player.IsScp && snapshot.HsRegen > 0)
                player.HumeShield = Math.Min(player.HumeShield + snapshot.HsRegen, player.MaxHumeShield);
            else if (!player.IsScp && snapshot.AhpRegen > 0
                     && EchoInfo.PlayerPassiveEffects.TryGetValue(player, out var state)
                     && TryGetEchoAhpProcess(player, state.EchoAhpKillCode, out var echoAhp))
            {
                echoAhp.CurrentAmount = Math.Min(echoAhp.CurrentAmount + snapshot.AhpRegen, echoAhp.Limit);
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }

    public static void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker == null || !EchoInfo.PlayerStats.TryGetValue(ev.Attacker, out var atkStats))
        {
            // defender-only path
        }
        else if (ev.Attacker != null && HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub))
        {
            float damage = ev.DamageHandler.Damage;

            damage *= 1f + atkStats.AttackPercent / 100f;
            damage += atkStats.AttackFlat;

            if (ev.Player.IsScp)
                damage *= 1f + atkStats.ScpDamagePercent / 100f;
            else
                damage *= 1f + atkStats.HumanDamagePercent / 100f;

            // Headshot: 기존 200%에 합산 적용 (ABattle BullsEye 참고)
            // PlayerStatsSystem.FirearmDamageHandler와 모호하므로 Exiled 타입을 명시
            if (atkStats.HeadshotDamage > 0
                && ev.DamageHandler.CustomBase is Exiled.API.Features.DamageHandlers.FirearmDamageHandler
                {
                    Hitbox: HitboxType.Headshot
                })
            {
                damage *= 1f + atkStats.HeadshotDamage / 200f;
            }

            // Critical (Ambush style)
            if (atkStats.CriticalChance > 0 && UnityEngine.Random.Range(0f, 100f) < atkStats.CriticalChance)
            {
                damage *= 2f;
                Timing.CallDelayed(Timing.WaitForOneFrame, () => ev.Attacker.ShowHitMarker(2));
            }

            ev.DamageHandler.Damage = damage;
        }

        if (EchoInfo.PlayerStats.TryGetValue(ev.Player, out var defStats)
            && !DefenseFlatIgnoredDamageTypes.Contains(ev.DamageHandler.Type))
        {
            // 방어력 정수: 고정 데미지 감소. 방어력%는 DamageReduction 이펙트로 처리.
            float dmg = ev.DamageHandler.Damage;
            dmg = Math.Max(0f, dmg - defStats.DefenseFlat);
            ev.DamageHandler.Damage = dmg;
        }
    }

    public static string GetMainStatDisplayName(EchoMainStatType type)
    {
        return type switch
        {
            EchoMainStatType.AttackPercent => "공격력%",
            EchoMainStatType.HpPercent => "HP%",
            EchoMainStatType.Defense => "방어력%",
            EchoMainStatType.ScpDamagePercent => "SCP 대상 데미지%",
            EchoMainStatType.HumanDamagePercent => "인간 대상 데미지%",
            EchoMainStatType.CriticalChance => "크리티컬 확률%",
            EchoMainStatType.MoveSpeedAndJump => "이동속도/점프력",
            EchoMainStatType.StaminaDrainReduction => "스태미나 소모 감소%",
            EchoMainStatType.HeadshotDamage => "헤드샷 데미지%",
            EchoMainStatType.AhpRegenAndMax => "AHP/HS 회복",
            _ => "?"
        };
    }

    public static string GetSubOptionDisplayName(EchoSubOptionType type)
    {
        return type switch
        {
            EchoSubOptionType.AttackPercent => "공격력%",
            EchoSubOptionType.AttackFlat => "공격력",
            EchoSubOptionType.DefensePercent => "방어력%",
            EchoSubOptionType.DefenseFlat => "방어력",
            EchoSubOptionType.HpPercent => "HP%",
            EchoSubOptionType.HpFlat => "HP",
            EchoSubOptionType.CriticalChance => "크리티컬%",
            EchoSubOptionType.ScpDamagePercent => "SCP 대상 데미지%",
            EchoSubOptionType.HumanDamagePercent => "인간 대상 데미지%",
            EchoSubOptionType.MoveSpeed => "이동속도%",
            EchoSubOptionType.JumpPower => "점프력%",
            EchoSubOptionType.StaminaDrainReduction => "스태미나 소모 감소%",
            EchoSubOptionType.HeadshotDamage => "헤드샷 데미지%",
            _ => "?"
        };
    }
}

public class EchoSubOptionInstance
{
    public EchoSubOptionType Type { get; set; }
    public int Grade { get; set; }
    public float Value { get; set; }
}

public class EchoStatSnapshot
{
    public float AttackPercent;
    public float AttackFlat;
    public float HpPercent;
    public float HpFlat;
    public float DefensePercent;
    public float DefenseFlat;
    public float ScpDamagePercent;
    public float HumanDamagePercent;
    public float CriticalChance;
    public float MoveSpeed;
    public float JumpPower;
    public float StaminaDrainReduction;
    public float HeadshotDamage;
    public float AhpRegen;
    public float AhpMax;
    public float HsRegen;
    public float HsMax;

    /// <summary>
    /// 같은 이름 스탯을 합산하여 표시용 딕셔너리로 반환합니다.
    /// </summary>
    public Dictionary<string, float> GetAggregatedDisplay()
    {
        var dict = new Dictionary<string, float>();

        void add(string name, float value)
        {
            if (Math.Abs(value) < 0.01f)
                return;
            if (dict.ContainsKey(name))
                dict[name] += value;
            else
                dict[name] = value;
        }

        add("공격력%", AttackPercent);
        add("공격력", AttackFlat);
        add("HP%", HpPercent);
        add("HP", HpFlat);
        add("방어력%", DefensePercent);
        add("방어력", DefenseFlat);
        add("SCP 대상 데미지%", ScpDamagePercent);
        add("인간 대상 데미지%", HumanDamagePercent);
        add("크리티컬 확률%", CriticalChance);
        add("이동속도%", MoveSpeed);
        add("점프력%", JumpPower);
        add("스태미나 소모 감소%", StaminaDrainReduction);
        add("헤드샷 데미지%", HeadshotDamage);
        add("AHP 회복", AhpRegen);
        add("AHP 최대", AhpMax);
        add("HS 회복", HsRegen);
        add("HS 최대", HsMax);

        return dict;
    }
}
