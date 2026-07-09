using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.EchoBattle;

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

    static readonly float[] SubOptionGradeWeights = { 250, 220, 190, 150, 110, 80 };

    static readonly Dictionary<EchoSubOptionType, float[]> SubOptionValues = new()
    {
        { EchoSubOptionType.AttackPercent, new[] { 6.5f, 7.4f, 8.3f, 9.2f, 10.1f, 11.0f } },
        { EchoSubOptionType.AttackFlat, new[] { 2f, 4f, 6f, 8f, 10f, 12f } },
        { EchoSubOptionType.DefensePercent, new[] { 2.0f, 2.5f, 3.0f, 3.5f, 4.0f, 4.5f } },
        { EchoSubOptionType.DefenseFlat, new[] { 3.0f, 3.4f, 3.8f, 4.2f, 4.6f, 5.0f } },
        { EchoSubOptionType.HpPercent, new[] { 8.1f, 9.2f, 10.3f, 11.4f, 12.5f, 13.6f } },
        { EchoSubOptionType.HpFlat, new[] { 20f, 26f, 32f, 38f, 44f, 50f } },
        { EchoSubOptionType.CriticalChance, new[] { 3.3f, 3.7f, 4.1f, 4.5f, 4.9f, 5.3f } },
        { EchoSubOptionType.ScpDamagePercent, new[] { 8.3f, 9.6f, 10.9f, 12.2f, 13.5f, 14.8f } },
        { EchoSubOptionType.HumanDamagePercent, new[] { 8.3f, 9.6f, 10.9f, 12.2f, 13.5f, 14.8f } },
        { EchoSubOptionType.MoveSpeed, new[] { 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f } },
        { EchoSubOptionType.JumpPower, new[] { 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f } },
        { EchoSubOptionType.StaminaDrainReduction, new[] { 8.3f, 9.2f, 10.1f, 11.0f, 11.9f, 12.8f } },
        { EchoSubOptionType.HeadshotDamage, new[] { 20.8f, 22.5f, 24.2f, 25.9f, 27.6f, 29.3f } },
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
            (EchoCost.Cost4, EchoMainStatType.ScpDamagePercent) => LerpStat(8.5f, 42.5f, level),
            (EchoCost.Cost4, EchoMainStatType.HumanDamagePercent) => LerpStat(8.5f, 42.5f, level),
            (EchoCost.Cost4, EchoMainStatType.CriticalChance) => LerpStat(4.4f, 22.0f, level),
            (EchoCost.Cost4, EchoMainStatType.MoveSpeedAndJump) => LerpStat(6.0f, 30.0f, level),

            (EchoCost.Cost3, EchoMainStatType.AttackPercent) => LerpStat(5.2f, 26.0f, level),
            (EchoCost.Cost3, EchoMainStatType.HpPercent) => LerpStat(8.6f, 43.0f, level),
            (EchoCost.Cost3, EchoMainStatType.Defense) => LerpStat(2.0f, 10.0f, level),
            (EchoCost.Cost3, EchoMainStatType.StaminaDrainReduction) => LerpStat(4.4f, 22.0f, level),
            (EchoCost.Cost3, EchoMainStatType.HeadshotDamage) => LerpStat(16.0f, 80.0f, level),
            (EchoCost.Cost3, EchoMainStatType.AhpRegenAndMax) => LerpStat(1.0f, 5.0f, level),

            (EchoCost.Cost1, EchoMainStatType.AttackPercent) => LerpStat(3.2f, 16.0f, level),
            (EchoCost.Cost1, EchoMainStatType.HpPercent) => LerpStat(4.8f, 24.0f, level),
            (EchoCost.Cost1, EchoMainStatType.Defense) => LerpStat(1.0f, 5.0f, level),

            _ => 0f
        };
    }

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

    public static List<EchoSubOptionInstance> GenerateSubOptions(int level, int? seed = null)
    {
        var result = new List<EchoSubOptionInstance>();
        int count = GetUnlockedSubOptionCount(level);
        var rng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        var types = Enum.GetValues(typeof(EchoSubOptionType)).Cast<EchoSubOptionType>()
            .Where(x => x != EchoSubOptionType.None).ToList();

        for (int i = 0; i < count; i++)
        {
            var optionType = types[rng.Next(types.Count)];
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
            var mainType = echo.Data.MainStatType;
            float mainValue = GetMainStatValue(cost, mainType, level);

            ApplyMainStat(snapshot, mainType, mainValue, cost, level);
            ApplySubStat(snapshot, cost, level, player);

            foreach (var option in echo.SubOptions)
                ApplySubOption(snapshot, option, player);
        }

        return snapshot;
    }

    static void ApplyMainStat(EchoStatSnapshot snapshot, EchoMainStatType type, float value, EchoCost cost, int level)
    {
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
                // Cost3 AHP: regen 1~5 / max 25~125 (SCP: HS regen 4~20 / max 140~700)
                snapshot.AhpRegen += value;
                snapshot.AhpMax += LerpStat(25f, 125f, level);
                snapshot.HsRegen += LerpStat(4f, 20f, level);
                snapshot.HsMax += LerpStat(140f, 700f, level);
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
                hp *= 11f;
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
                snapshot.HpFlat += player.IsScp ? option.Value * 11f : option.Value;
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

    public static void ApplyPassiveEffects(Player player, EchoStatSnapshot snapshot)
    {
        // HP
        float baseMax = player.MaxHealth;
        float newMax = baseMax * (1f + snapshot.HpPercent / 100f) + snapshot.HpFlat;
        float ratio = player.Health / Math.Max(1f, player.MaxHealth);
        player.MaxHealth = newMax;
        player.Health = Mathf.Clamp(newMax * ratio, 1f, newMax);

        // Defense % via DamageReduction effect (intensity ≈ percent * 2, Rank 방어 참고)
        if (snapshot.DefensePercent > 0)
            player.AddEffect(EffectType.DamageReduction, (byte)Mathf.Clamp(Mathf.RoundToInt(snapshot.DefensePercent * 2f), 1, 255));

        if (snapshot.MoveSpeed > 0)
            player.AddEffect(EffectType.MovementBoost, (byte)Mathf.Clamp(Mathf.RoundToInt(snapshot.MoveSpeed), 1, 255));

        // 점프력: Lightweight 효과로 구현 (Rank 예능 참고)
        if (snapshot.JumpPower > 0)
            player.AddEffect(EffectType.Lightweight, (byte)Mathf.Clamp(Mathf.RoundToInt(snapshot.JumpPower), 1, 255));

        // AHP / HS
        if (player.IsScp)
        {
            if (snapshot.HsMax > 0)
            {
                player.MaxHumeShield = Math.Max(player.MaxHumeShield, snapshot.HsMax);
                player.HumeShield = Math.Min(player.HumeShield + snapshot.HsMax * 0.1f, player.MaxHumeShield);
            }
        }
        else if (snapshot.AhpMax > 0)
        {
            player.MaxArtificialHealth = Math.Max(player.MaxArtificialHealth, snapshot.AhpMax);
            player.ArtificialHealth = Math.Min(player.ArtificialHealth + snapshot.AhpMax * 0.1f, player.MaxArtificialHealth);
        }

        if (snapshot.AhpRegen > 0 || snapshot.HsRegen > 0)
            Timing.RunCoroutine(RegenRoutine(player, snapshot), $"EchoRegen_{player.UserId}");
    }

    static IEnumerator<float> RegenRoutine(Player player, EchoStatSnapshot snapshot)
    {
        while (player != null && player.IsAlive && EchoInfo.PlayerStats.ContainsKey(player))
        {
            if (player.IsScp && snapshot.HsRegen > 0)
                player.HumeShield = Math.Min(player.HumeShield + snapshot.HsRegen, player.MaxHumeShield);
            else if (!player.IsScp && snapshot.AhpRegen > 0)
                player.ArtificialHealth = Math.Min(player.ArtificialHealth + snapshot.AhpRegen, player.MaxArtificialHealth);

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
            if (atkStats.HeadshotDamage > 0
                && ev.DamageHandler.CustomBase is FirearmDamageHandler firearm
                && firearm.Hitbox == HitboxType.Headshot)
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

        if (EchoInfo.PlayerStats.TryGetValue(ev.Player, out var defStats))
        {
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
