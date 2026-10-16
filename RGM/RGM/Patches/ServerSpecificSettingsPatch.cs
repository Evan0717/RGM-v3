using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using HarmonyLib;
using UserSettings.ServerSpecific;

namespace RGM.Patches;

/// <summary>
/// 여러 플러그인이 같은 설정을 재등록하더라도 서버 설정 ID가 중복되지 않도록 보정합니다.
/// </summary>
public static class ServerSpecificSettingsPatch
{
    public static void Apply(Harmony harmony)
    {
        harmony.Patch(
            AccessTools.PropertySetter(typeof(ServerSpecificSettingsSync),
                nameof(ServerSpecificSettingsSync.DefinedSettings)),
            prefix: new HarmonyMethod(typeof(ServerSpecificSettingsPatch), nameof(RemoveDuplicateSettingIds)));
    }

    private static void RemoveDuplicateSettingIds(ref ServerSpecificSettingBase[] value)
    {
        if (value is not { Length: > 1 })
            return;

        var registeredIds = new HashSet<int>();
        var uniqueSettings = new List<ServerSpecificSettingBase>(value.Length);
        var duplicateIds = new List<int>();

        foreach (ServerSpecificSettingBase setting in value)
        {
            if (setting != null && registeredIds.Add(setting.SettingId))
                uniqueSettings.Add(setting);
            else
                duplicateIds.Add(setting?.SettingId ?? 0);
        }

        if (duplicateIds.Count == 0)
            return;

        value = uniqueSettings.ToArray();
        Log.Warn($"중복된 서버 설정 ID {duplicateIds.Count}개를 제거했습니다: " +
                 string.Join(", ", duplicateIds));
    }
}
