using HarmonyLib;
using PlayerStatsSystem;

namespace RGM.Patches;

/// <summary>
/// HP, AHP, HS가 네트워크 및 내부 연산 한계를 넘지 않도록 제한합니다.
/// </summary>
public static class HpAndShieldLimit
{
    public const float MaxHealth = 16_777_215f;
    public const float MaxShield = ushort.MaxValue;

    public static void Apply(Harmony harmony)
    {
        // HealthStat와 AhpStat은 CurValue 설정자를 SyncedStatBase에서 공유합니다.
        harmony.Patch(
            AccessTools.PropertySetter(typeof(SyncedStatBase), nameof(SyncedStatBase.CurValue)),
            prefix: new HarmonyMethod(typeof(HpAndShieldLimit), nameof(ClampSyncedCurrentValue)));

        harmony.Patch(
            AccessTools.PropertySetter(typeof(HumeShieldStat), nameof(HumeShieldStat.CurValue)),
            prefix: new HarmonyMethod(typeof(HpAndShieldLimit), nameof(ClampHumeShieldCurrentValue)));

        PatchMaxValue(harmony, typeof(HealthStat), nameof(ClampHealthMaxValue));
        PatchMaxValue(harmony, typeof(AhpStat), nameof(ClampShieldMaxValue));
        PatchMaxValue(harmony, typeof(HumeShieldStat), nameof(ClampShieldMaxValue));

        // AHP 프로세스 자체도 제한하여 누적값이 한도를 넘어가는 것을 방지합니다.
        harmony.Patch(
            AccessTools.Method(typeof(AhpStat), nameof(AhpStat.ServerAddProcess),
                [typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(bool)]),
            prefix: new HarmonyMethod(typeof(HpAndShieldLimit), nameof(ClampAhpProcess)));
    }

    private static void PatchMaxValue(Harmony harmony, System.Type statType, string prefixName)
    {
        harmony.Patch(
            AccessTools.PropertySetter(statType, nameof(StatBase.MaxValue)),
            prefix: new HarmonyMethod(typeof(HpAndShieldLimit), prefixName));
    }

    private static void ClampSyncedCurrentValue(SyncedStatBase __instance, ref float value)
    {
        value = __instance switch
        {
            HealthStat => System.Math.Min(value, MaxHealth),
            AhpStat => System.Math.Min(value, MaxShield),
            _ => value,
        };
    }

    private static void ClampHumeShieldCurrentValue(ref float value) =>
        value = System.Math.Min(value, MaxShield);

    private static void ClampHealthMaxValue(ref float value) =>
        value = System.Math.Min(value, MaxHealth);

    private static void ClampShieldMaxValue(ref float value) =>
        value = System.Math.Min(value, MaxShield);

    private static void ClampAhpProcess(ref float amount, ref float limit)
    {
        amount = System.Math.Min(amount, MaxShield);
        limit = System.Math.Min(limit, MaxShield);
    }
}
