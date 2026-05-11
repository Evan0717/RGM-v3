using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using RGM.API.Features;

namespace RGM.Modes;

public abstract class StoreItem
{
    public abstract void OnEnabled();

    public abstract void OnDisabled();

    public StoreItemData Data { get; set; }
    public Player Owner { get; set; }
}

public class StoreItemData
{
    public Type Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public StoreItemLevel Level { get; set; }
    public StoreItemType StoreItemType { get; set; }
    public StoreItemHolidayType HolidayType { get; set; }

    public string GetFormattedName()
    {
        return $"<color={Level.GetColor()}>[{Level.GetTranslation()}]</color> {Name}";
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class StoreItemAttribute(string name, string description, StoreItemLevel level, StoreItemType type, StoreItemHolidayType holidayType = StoreItemHolidayType.None) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public StoreItemLevel Level { get; set; } = level;
    public StoreItemType Type { get; set; } = type;
    public StoreItemHolidayType HolidayType { get; set; } = holidayType;
}

public static class StoreItemLevelExtensions
{
    public static string GetTranslation(this StoreItemLevel level)
    {
        return level switch
        {
            StoreItemLevel.Starter => "시작 아이템",
            StoreItemLevel.Basic => "기본 아이템",
            StoreItemLevel.Component => "보조 아이템",
            StoreItemLevel.Completed => "완성 아이템",
            StoreItemLevel.Prism => "프리즘 아이템",
            _ => "?"
        };
    }
    
    public static string GetColor(this StoreItemLevel level)
    {
        return level switch
        {
            StoreItemLevel.Starter => "#53d9c9",
            StoreItemLevel.Basic => "#b7b7b7",
            StoreItemLevel.Component => "#2ECCFA",
            StoreItemLevel.Completed => "#FF00FF",
            StoreItemLevel.Prism => "#ffd700",
            _ => "white"
        };
    }
}

public static class StorePlayerExtensions 
{
    public static int StoreItemCount(this Player player, StoreItemType StoreItemType)
    {
        return Store.Instance.PlayerStoreItems[player].Count(x => x.Data.StoreItemType == StoreItemType);
    }
}

public enum StoreItemHolidayType
{
    None,
    Christmas,
    Halloween,
}

public enum StoreItemLevel
{
    None,
    Starter, // 시작 아이템
    Basic, // 기본 아이템
    Component, // 보조 아이템
    Completed, // 완성 아이템
    Prism // 프리즘 아이템
}

public enum StoreItemType
{
    None,

    // 시작 아이템
    기본인간의_의지,

}

public static class StoreItemTypeExtensions
{
    public static string GetTranslation(this StoreItemType type)
    {
        var store = Store.Instance;

        if (!store.StoreItems.TryGetValue(type, out var StoreItem))
            return "?";

        return StoreItem.GetFormattedName();
    }
}