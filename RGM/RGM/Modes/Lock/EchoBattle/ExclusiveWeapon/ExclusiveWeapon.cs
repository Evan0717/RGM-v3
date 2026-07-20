using Exiled.API.Features;
using System;

namespace RGM.Modes;

/// <summary>
/// EchoBattle 전용무기. 능력치 레벨(1~90)과 공진(패시브) 레벨(1~5)을 가집니다.
/// (클래스명 ExcWeapon: 네임스페이스 ExclusiveWeapon과의 충돌 방지)
/// </summary>
public abstract class ExcWeapon
{
    public ExclusiveWeaponData Data { get; set; }
    public Player Owner { get; set; }
    public int Level { get; set; } = 1;
    public int Resonance { get; set; } = 1;

    public abstract float AttackFlatMin { get; }
    public abstract float AttackFlatMax { get; }
    public abstract ExclusiveWeaponSecondaryStat SecondaryStat { get; }
    public abstract float SecondaryStatMin { get; }
    public abstract float SecondaryStatMax { get; }

    /// <summary>패시브 공격력% 보너스. 없으면 0.</summary>
    public virtual float PassiveAttackPercent => 0f;

    /// <summary>패시브 HP% 보너스. 없으면 0.</summary>
    public virtual float PassiveHpPercent => 0f;

    /// <summary>패시브 크리티컬 확률% 보너스. 없으면 0.</summary>
    public virtual float PassiveCriticalChance => 0f;

    public abstract void OnEnabled();
    public abstract void OnDisabled();

    public virtual void ContributeStats(EchoStatSnapshot snapshot, bool includeAttackStats = true)
    {
        if (snapshot == null)
            return;

        if (includeAttackStats)
            snapshot.AttackFlat += ExclusiveWeaponStats.LerpStat(AttackFlatMin, AttackFlatMax, Level);

        float secondary = ExclusiveWeaponStats.LerpStat(SecondaryStatMin, SecondaryStatMax, Level);
        switch (SecondaryStat)
        {
            case ExclusiveWeaponSecondaryStat.CriticalChance:
                if (includeAttackStats)
                    snapshot.CriticalChance += secondary;
                break;
            case ExclusiveWeaponSecondaryStat.HpPercent:
                snapshot.HpPercent += secondary;
                break;
        }

        if (includeAttackStats)
        {
            snapshot.AttackPercent += PassiveAttackPercent;
            snapshot.CriticalChance += PassiveCriticalChance;
        }

        snapshot.HpPercent += PassiveHpPercent;
    }

    /// <summary>대상 공격 시 추가 크리티컬 데미지%(기본 200%에 가산). 기본 0.</summary>
    public virtual float GetCriticalDamageBonus(Player target) => 0f;
}

public class ExclusiveWeaponData
{
    public Type Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ExclusiveWeaponType WeaponType { get; set; }

    public string GetFormattedName() => Name;
}

[AttributeUsage(AttributeTargets.Class)]
public class ExclusiveWeaponAttribute(
    string name,
    string description,
    ExclusiveWeaponType type) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public ExclusiveWeaponType Type { get; } = type;
}

public static class ExclusiveWeaponTypeExtensions
{
    public static ExclusiveWeaponData GetData(this ExclusiveWeaponType type)
    {
        return ExclusiveWeaponInfo.Weapons.TryGetValue(type, out var data) ? data : null;
    }
}

public enum ExclusiveWeaponType
{
    None,
    SpectrumBlaster,
    NightskyCalculator,
    ForgedStar,
    FrostBurn,
    KumoKiri,
    DeathFlower
}

public enum ExclusiveWeaponSecondaryStat
{
    None,
    CriticalChance,
    HpPercent
}
