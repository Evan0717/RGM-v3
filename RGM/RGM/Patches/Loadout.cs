using Exiled.API.Features;
using InventorySystem;
using InventorySystem.Configs;
using PlayerRoles;

namespace RGM.Patches;

/// <summary>
/// 역할별 기본 장비 구성을 조정합니다.
/// </summary>
public static class LoadoutPatch
{
    /// <summary>
    /// Facility Guard의 기본 장비에서 FSP-9를 CrossVec으로 교체합니다.
    /// </summary>
    public static void Apply()
    {
        if (!StartingInventories.DefinedInventories.TryGetValue(RoleTypeId.FacilityGuard,
                out InventoryRoleInfo facilityGuardLoadout))
        {
            Log.Error("[LoadoutPatch] Facility Guard의 기본 장비를 찾지 못했습니다.");
            return;
        }

        ItemType[] items = (ItemType[])facilityGuardLoadout.Items.Clone();
        bool replaced = false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != ItemType.GunFSP9)
                continue;

            items[i] = ItemType.GunCrossvec;
            replaced = true;
        }

        if (!replaced)
        {
            Log.Warn("[LoadoutPatch] Facility Guard 기본 장비에 FSP-9가 없습니다.");
            return;
        }

        // 기본 장비를 조회하는 모든 경로가 이 정의를 사용하므로, 아이템 지급 전에 교체됩니다.
        StartingInventories.DefinedInventories[RoleTypeId.FacilityGuard] =
            new InventoryRoleInfo(items, facilityGuardLoadout.Ammo);

        Log.Info("[LoadoutPatch] Facility Guard loadout weapon replaced with CrossVec.");
    }
}
