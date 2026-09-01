using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Firearms.Modules.Scp127;
using System;

namespace RGM.Patches;

/// <summary>
/// 무기 전역 패치:
/// 1) 리볼버 거리 감쇠 제거
/// 2) SCP-127 거리 감쇠를 성장 단계에 따라 완화 (1단계 유지 / 2단계 30m 증가 / 3단계 무제한)
///    감쇠 거리는 사격 시 레이캐스트 최대 거리(감쇠 거리 + 최대 피해 거리)도 함께 결정하므로,
///    값을 늘리면 사거리 제한까지 같이 풀립니다.
/// </summary>
public static class WeaponPatch
{
    /// <summary>거리 감쇠를 사실상 제거하는 값입니다.</summary>
    private const float UnlimitedFalloffDistance = 999f;

    private const float Scp127Tier2FalloffBonus = 30f;

    public static void Apply(Harmony harmony)
    {
        try
        {
            // 총기 인스턴스마다 값을 덮어쓰면 지급·습득·SCP-914 등 모든 획득 경로를 따라다녀야 하므로,
            // 조회 지점 한 곳만 보정합니다.
            harmony.Patch(
                AccessTools.PropertyGetter(typeof(HitscanHitregModuleBase),
                    nameof(HitscanHitregModuleBase.DamageFalloffDistance)),
                postfix: new HarmonyMethod(typeof(WeaponPatch), nameof(DamageFalloffDistanceGetterPostfix)));

            Log.Info("[WeaponPatch] Applied.");
        }
        catch (Exception e)
        {
            Log.Error($"[WeaponPatch] Failed to apply: {e}");
        }
    }

    public static void DamageFalloffDistanceGetterPostfix(HitscanHitregModuleBase __instance, ref float __result)
    {
        try
        {
            Firearm firearm = __instance.Firearm;

            if (firearm == null)
                return;

            switch (firearm.ItemTypeId)
            {
                case ItemType.GunRevolver:
                    __result = UnlimitedFalloffDistance;
                    break;

                case ItemType.GunSCP127:
                    __result = GetScp127FalloffDistance(firearm, __result);
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error($"[WeaponPatch] DamageFalloffDistanceGetterPostfix Exception: {e}");
        }
    }

    private static float GetScp127FalloffDistance(Firearm firearm, float baseFalloffDistance)
    {
        // 성장 단계는 총기 소유자별로 기록되므로, 주인이 없는 동안에는 기본값을 사용합니다.
        if (firearm.Owner == null)
            return baseFalloffDistance;

        return Scp127TierManagerModule.GetTierForItem(firearm) switch
        {
            Scp127Tier.Tier2 => baseFalloffDistance + Scp127Tier2FalloffBonus,
            Scp127Tier.Tier3 => UnlimitedFalloffDistance,
            _ => baseFalloffDistance,
        };
    }
}
