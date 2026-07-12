using System;

namespace RGM.Modes;

/// <summary>전용무기 능력치 레벨 보간 및 수치 유틸.</summary>
public static class ExclusiveWeaponStats
{
    public static float LerpStat(float min, float max, int level)
    {
        level = Clamp(level, 1, ExclusiveWeaponInfo.MaxLevel);
        float t = (level - 1) / (float)(ExclusiveWeaponInfo.MaxLevel - 1);
        return min + (max - min) * t;
    }

    public static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}
