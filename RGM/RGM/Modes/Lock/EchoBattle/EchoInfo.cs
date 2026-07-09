using Exiled.API.Features;
using System;
using System.Collections.Generic;

namespace RGM.RGM.Modes.Lock.EchoBattle;

public static class EchoInfo
{
    public const int MaxEquippedEchoes = 5;
    public const int MaxTotalCost = 12;
    public const int MaxLevel = 25;
    public const int ApplyDelaySeconds = 30;

    public static Dictionary<EchoType, EchoData> Echoes = new();
    public static Dictionary<Player, EchoLoadout> PlayerLoadouts = new();
    public static Dictionary<Player, List<Echo>> PlayerEchoes = new();
    public static Dictionary<Player, EchoStatSnapshot> PlayerStats = new();
    public static Dictionary<Player, bool> PlayerShowHints = new();

    public static readonly Dictionary<EchoType, (string Name, string Description)> EchoCatalog = new()
    {
        { EchoType.Gnome, ("노움", "사용 시 10초간 데미지 감소 40%, 재사용 대기시간 60초") },
        { EchoType.Salamandra, ("살라만드라", "사용 시 자신이 공격하는 모든 공격에 화상 효과 적용. 지속 시간 30초, 재사용 대기시간 60초") },
        { EchoType.Undine, ("운디네", "사용 시 주변 15m 내에 있는 모든 상대 3초간 40% 감속, 재사용 대기시간 60초") },
        { EchoType.Sylph, ("실프", "12초간 이동 속도 50% 증가 및 Ghostly, Fade 효과 적용, 재사용 대기시간 60초") },
        { EchoType.Capybara, ("카피바라", "사용 시 20초간 최대 체력의 3%씩 회복, 재사용 대기시간 60초") },
        { EchoType.Berserker, ("광전사", "사용 시 다음 공격은 자신의 최대 체력의 50%만큼 추가 데미지, 재사용 대기시간 60초") },
        { EchoType.PetitDangdang, ("쁘띠 댕댕이", "사용 시 15초간 시야 개선 및 스태미너 무제한, 재사용 대기시간 60초") },
        { EchoType.GoldenPig, ("황금 돼지", "사용 시 체력 1205 회복(최대치 초과 불가), 재사용 대기시간 60초") },
        { EchoType.ClassD, ("ClassD", "Cost 1 샘플 Echo") },
        { EchoType.Scientist, ("과학자", "Cost 1 샘플 Echo") },
        { EchoType.FacilityGuard, ("시설 경비", "Cost 1 샘플 Echo") },
    };

    public static IEnumerable<EchoType> GetEchoesByCost(EchoCost cost)
    {
        foreach (var pair in Echoes)
        {
            if (pair.Value.Cost == cost)
                yield return pair.Key;
        }
    }
}

public class EchoLoadout
{
    public EchoType? MainSlot { get; set; }
    public EchoType?[] SubSlots { get; set; } = new EchoType?[4];
    public Dictionary<EchoType, int> Levels { get; set; } = new();
    public Dictionary<EchoType, int> Experience { get; set; } = new();

    public IEnumerable<EchoType> GetEquipped()
    {
        if (MainSlot.HasValue)
            yield return MainSlot.Value;

        foreach (var slot in SubSlots)
        {
            if (slot.HasValue)
                yield return slot.Value;
        }
    }

    public int GetTotalCost()
    {
        int total = 0;
        foreach (var type in GetEquipped())
        {
            var data = type.GetData();
            if (data != null)
                total += (int)data.Cost;
        }

        return total;
    }

    public int GetEquippedCount()
    {
        int count = 0;
        foreach (var _ in GetEquipped())
            count++;
        return count;
    }

    public int GetLevel(EchoType type)
    {
        if (Levels.TryGetValue(type, out int level))
            return EchoStats.Clamp(level, 1, EchoInfo.MaxLevel);

        return 1;
    }

    public bool Contains(EchoType type)
    {
        if (MainSlot == type)
            return true;

        foreach (var slot in SubSlots)
        {
            if (slot == type)
                return true;
        }

        return false;
    }
}
