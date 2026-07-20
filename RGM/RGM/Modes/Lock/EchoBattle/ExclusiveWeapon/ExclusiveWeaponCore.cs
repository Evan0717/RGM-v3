using Exiled.API.Features;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RGM.Modes;

public static class ExclusiveWeaponCore
{
    public static void RegisterWeapons()
    {
        ExclusiveWeaponInfo.Weapons.Clear();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var attr = type.GetCustomAttribute<ExclusiveWeaponAttribute>();
            if (attr == null || !typeof(ExcWeapon).IsAssignableFrom(type))
                continue;

            ExclusiveWeaponInfo.Weapons[attr.Type] = new ExclusiveWeaponData
            {
                Type = type,
                Name = attr.Name,
                Description = attr.Description,
                WeaponType = attr.Type
            };
        }

        Log.Info($"[EchoBattle] Registered {ExclusiveWeaponInfo.Weapons.Count} exclusive weapons.");
    }

    public static void ApplyWeapon(Player player)
    {
        RemoveWeapon(player);

        if (!EchoInfo.PlayerLoadouts.TryGetValue(player, out var loadout) || !loadout.EquippedWeapon.HasValue)
        {
            ExclusiveWeaponQuest.StopTracking(player);
            return;
        }

        var weaponType = loadout.EquippedWeapon.Value;
        if (!ExclusiveWeaponInfo.Weapons.TryGetValue(weaponType, out var data))
        {
            Log.Error($"[EchoBattle] Exclusive weapon {weaponType} not found.");
            return;
        }

        ExcWeapon weapon;
        try
        {
            weapon = Activator.CreateInstance(data.Type) as ExcWeapon;
        }
        catch (Exception e)
        {
            Log.Error($"[EchoBattle] Failed to create weapon {data.Name}: {e}");
            return;
        }

        if (weapon == null)
            return;

        var progress = ExclusiveWeaponInfo.GetOrCreateProgress(player);
        weapon.Data = data;
        weapon.Owner = player;
        weapon.Level = progress.GetLevel(weaponType);
        weapon.Resonance = progress.GetResonance(weaponType);

        ExclusiveWeaponInfo.PlayerWeapons[player] = weapon;
        weapon.OnEnabled();
        ExclusiveWeaponQuest.EnsureTracking(player);
    }

    public static void RemoveWeapon(Player player)
    {
        if (player == null)
            return;

        if (ExclusiveWeaponInfo.PlayerWeapons.TryGetValue(player, out var weapon))
        {
            weapon.OnDisabled();
            ExclusiveWeaponInfo.PlayerWeapons.Remove(player);
        }
    }

    public static void MergeStats(Player player, EchoStatSnapshot snapshot)
    {
        if (player == null || snapshot == null)
            return;

        if (!ExclusiveWeaponInfo.PlayerWeapons.TryGetValue(player, out var weapon) || weapon == null)
            return;

        weapon.ContributeStats(snapshot, !EchoStats.AreAttackModifiersIgnored(player));
    }

    public static void Reset(Player player)
    {
        ExclusiveWeaponQuest.StopTracking(player);
        RemoveWeapon(player);
        ExclusiveWeaponGrowth.ClearPending(player);
    }

    public static void ClearAll(Player player)
    {
        Reset(player);
        ExclusiveWeaponQuest.ClearPlayer(player);
        ExclusiveWeaponInfo.ClearPlayer(player);
    }

    public static IEnumerable<ExclusiveWeaponData> GetAllOrdered()
    {
        return ExclusiveWeaponInfo.Weapons.Values.OrderBy(x => x.Name);
    }

    public static string BuildHintSection(Player player)
    {
        if (!ExclusiveWeaponInfo.PlayerWeapons.TryGetValue(player, out var weapon) || weapon?.Data == null)
            return null;

        var lines = new List<string>
        {
            "<color=#ffcc66>── 전용무기 ──</color>",
            $"<b>{weapon.Data.Name}</b> Lv.{weapon.Level} 공진 {weapon.Resonance}"
        };

        lines.Add($"XP {ExclusiveWeaponGrowth.FormatProgress(player, weapon.Data.WeaponType)}");
        lines.Add($"공진: {ExclusiveWeaponQuest.FormatCurrentQuest(player, weapon.Data.WeaponType)}");

        float atk = ExclusiveWeaponStats.LerpStat(weapon.AttackFlatMin, weapon.AttackFlatMax, weapon.Level);
        lines.Add($"공격력 +{atk:0.#}");

        float secondary = ExclusiveWeaponStats.LerpStat(weapon.SecondaryStatMin, weapon.SecondaryStatMax, weapon.Level);
        if (weapon.SecondaryStat == ExclusiveWeaponSecondaryStat.CriticalChance)
            lines.Add($"크리티컬 +{secondary:0.#}%");
        else if (weapon.SecondaryStat == ExclusiveWeaponSecondaryStat.HpPercent)
            lines.Add($"HP +{secondary:0.#}%");

        if (weapon.PassiveAttackPercent > 0.01f)
            lines.Add($"패시브 공격력 +{weapon.PassiveAttackPercent:0.#}%");
        if (weapon.PassiveHpPercent > 0.01f)
            lines.Add($"패시브 HP +{weapon.PassiveHpPercent:0.#}%");
        if (weapon.PassiveCriticalChance > 0.01f)
            lines.Add($"패시브 크리티컬 +{weapon.PassiveCriticalChance:0.#}%");

        return string.Join("\n", lines);
    }
}
