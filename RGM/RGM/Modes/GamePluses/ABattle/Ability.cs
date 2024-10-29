using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace RGM.Modes;

public abstract class Ability
{
    public abstract void OnEnabled();

    public abstract void OnDisabled();

    public AbilityData Data { get; set; }
    public Player Owner { get; set; }
}

public abstract class EffectAbility : Ability
{
    public abstract Dictionary<EffectType, byte> EffectTypes { get; }

    public override void OnEnabled()
    {
        foreach (var effect in EffectTypes)
        {
            Owner.AddEffect(effect.Key, effect.Value);
        }
    }

    public override void OnDisabled()
    {
        foreach (var effect in EffectTypes)
        {
            Owner.RemoveEffect(effect.Key, effect.Value);
        }
    }
}

public abstract class ItemAbility : Ability
{
    public abstract ItemType ItemType { get; }
    public abstract int Amount { get; }

    public override void OnEnabled()
    {
        Owner.AddItem(ItemType, Amount);
    }

    public override void OnDisabled()
    {
        for (int i = 0; i < Amount; i++)
        {
            var item = Owner.Items.FirstOrDefault(x => x.Type == ItemType);

            if (item == null)
                break;

            Owner.RemoveItem(item);
        }
    }
}

public class AbilityData
{
    public Type Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public AbilityCategory Category { get; set; }
    public AbilityType AbilityType { get; set; }
    public List<AbilityType> Requires { get; set; }
    public bool Keep { get; set; }

    public string GetFormattedName()
    {
        return $"<color={Category.GetColor()}>[{Category.GetTranslation()}]</color> {Name}";
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class AbilityAttribute(string name, string description, AbilityCategory category, AbilityType type, bool keep = false) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public AbilityCategory Category { get; } = category;
    public AbilityType Type { get; set; } = type;
    public bool Keep { get; set; } = false;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequiresAbilityAttribute(params AbilityType[] abilities) : Attribute
{
    public AbilityType[] Abilities { get; } = abilities;
}

public enum AbilityCategory
{
    None,
    Common,
    Rare,
    Epic,
    Legend,
    Mythic,
    ClassD,
    Scientist,
    Guard,
    Ntf,
    Chaos,
    Snake,
    Scp173,
    Scp049,
    Scp0492,
    Scp096,
    Scp106,
    Scp939,
    Scp3114,
    Scp079,
    Synergy,
}

public static class AbilityCategoryExtensions
{
    public static string GetTranslation(this AbilityCategory category)
    {
        return category switch
        {
            AbilityCategory.Common => "일반",
            AbilityCategory.Rare => "희귀",
            AbilityCategory.Epic => "영웅",
            AbilityCategory.Legend => "전설",
            AbilityCategory.Mythic => "신화",
            AbilityCategory.ClassD => "전용",
            AbilityCategory.Scientist => "전용",
            AbilityCategory.Guard => "전용",
            AbilityCategory.Ntf => "전용",
            AbilityCategory.Chaos => "전용",
            AbilityCategory.Snake => "전용",
            AbilityCategory.Scp173 => "전용",
            AbilityCategory.Scp049 => "전용",
            AbilityCategory.Scp0492 => "전용",
            AbilityCategory.Scp096 => "전용",
            AbilityCategory.Scp106 => "전용",
            AbilityCategory.Scp939 => "전용",
            AbilityCategory.Scp3114 => "전용",
            AbilityCategory.Scp079 => "전용",
            AbilityCategory.Synergy => "시너지",
            _ => "알 수 없음"
        };
    }
    
    public static string GetColor(this AbilityCategory category)
    {
        return category switch
        {
            AbilityCategory.Common => "#A4A4A4",
            AbilityCategory.Rare => "#2ECCFA",
            AbilityCategory.Epic => "#FF00FF",
            AbilityCategory.Legend => "#ffd700",
            AbilityCategory.Mythic => "#DF0101",
            AbilityCategory.ClassD => "#F7819F",
            AbilityCategory.Scientist => "#F7819F",
            AbilityCategory.Guard => "#F7819F",
            AbilityCategory.Ntf => "#F7819F",
            AbilityCategory.Chaos => "#F7819F",
            AbilityCategory.Snake => "#F7819F",
            AbilityCategory.Scp173 => "#F7819F",
            AbilityCategory.Scp049 => "#F7819F",
            AbilityCategory.Scp0492 => "#F7819F",
            AbilityCategory.Scp096 => "#F7819F",
            AbilityCategory.Scp106 => "#F7819F",
            AbilityCategory.Scp939 => "#F7819F",
            AbilityCategory.Scp3114 => "#F7819F",
            AbilityCategory.Scp079 => "#F7819F",
            AbilityCategory.Synergy => "#DEEFED",
            _ => "white"
        };
    }
}

public enum AbilityType
{
    NONE,
    NORMAL_WORKOUT,
    NORMAL_SWIFT,
    RARE_TRANSITION,
    EPIC_TRANSITION,
    LEGEND_TRANSITION,
    MYTHIC_JOKER
}

public static class AbilityTypeExtensions
{
    public static string GetTranslation(this AbilityType type)
    {
        var aBattle = ABattle.Instance;

        if (!aBattle.Abilities.TryGetValue(type, out var ability))
            return "알 수 없음";

        return ability.GetFormattedName();
    }
}