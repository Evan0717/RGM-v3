using DAONTFT.Core.TFT;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;

using static RGM.Modes.RankVar;

namespace RGM.Modes;

public abstract class RankAbility
{
    public abstract void OnEnabled();

    public abstract void OnDisabled();

    public RankAbilityData Data { get; set; }
    public Player Owner { get; set; }
}

public class RankAbilityData
{
    public Type Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Emoji { get; set; } = "🎲";
    public RankAbilityType TFTAbilityType { get; set; }

    public string GetFormattedName()
    {
        return $"<color={Level.GetColor()}>[{Level.GetTranslation()}]</color> {Emoji} {Name}";
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class RankAbilityAttribute(string name, string description, RankAbilityType type, string emoji = "🎲") : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string Emoji { get; set; } = emoji;
    public RankAbilityType Type { get; set; } = type;
}

public static class RankAbilityTypeExtensions
{
    public static RankAbilityData GetData(RankAbilityType type)
    {
        if (!RankAbilities.TryGetValue(type, out var RankAbility))
            return null;

        return RankAbility;
    }
}

public static class RankAbilityCategoryExtensions
{
    public static string GetTranslation(this RankAbilityCategory category)
    {
        return category switch
        {
            RankAbilityCategory.변칙성 => "변칙성",
            RankAbilityCategory.가젯 => "가젯",
            RankAbilityCategory.기어 => "기어",
            _ => "?"
        };
    }

    public static string GetColor(this RankAbilityCategory category)
    {
        return category switch
        {
            RankAbilityCategory.변칙성 => "#e4ea4c",
            RankAbilityCategory.가젯 => "#78f168",
            RankAbilityCategory.기어 => "#91edd3",
            _ => "white"
        };
    }
}

public static class RankCategoryExtensions
{
    public static RankCategory GetAbilityCategory(this Player player)
    {
        RoleTypeId role = player.Role.Type;

        return role switch
        {
            RoleTypeId.ClassD => RankCategory.D계급,
            RoleTypeId.Scientist => RankCategory.과학자,
            RoleTypeId.FacilityGuard => RankCategory.시설_경비,
            RoleTypeId.NtfPrivate => RankCategory.구미호,
            RoleTypeId.NtfSergeant => RankCategory.구미호,
            RoleTypeId.NtfCaptain => RankCategory.구미호,
            RoleTypeId.NtfSpecialist => RankCategory.구미호,
            RoleTypeId.ChaosRifleman => RankCategory.반란,
            RoleTypeId.ChaosMarauder => RankCategory.반란,
            RoleTypeId.ChaosRepressor => RankCategory.반란,
            RoleTypeId.ChaosConscript => RankCategory.반란,
            RoleTypeId.Scp173 => RankCategory.SCP_173,
            RoleTypeId.Scp049 => RankCategory.SCP_049,
            RoleTypeId.Scp0492 => RankCategory.SCP_049_2,
            RoleTypeId.Scp096 => RankCategory.SCP_096,
            RoleTypeId.Scp106 => RankCategory.SCP_106,
            RoleTypeId.Scp939 => RankCategory.SCP_939,
            RoleTypeId.Scp3114 => RankCategory.SCP_3114,
            RoleTypeId.Scp079 => RankCategory.SCP_079,
            _ => RankCategory.튜토리얼,
        };
    }

    public static string GetTranslation(this RankCategory category)
    {
        return category switch
        {
            RankCategory.D계급 => "D계급",
            RankCategory.과학자 => "과학자",
            RankCategory.시설_경비 => "시설 경비",
            RankCategory.구미호 => "구미호",
            RankCategory.반란 => "반란",
            RankCategory.튜토리얼 => "튜토리얼",
            RankCategory.SCP_049 => "SCP-049",
            RankCategory.SCP_049_2 => "SCP-049-2",
            RankCategory.SCP_079 => "SCP-079",
            RankCategory.SCP_096 => "SCP-096",
            RankCategory.SCP_106 => "SCP-106",
            RankCategory.SCP_173 => "SCP-173",
            RankCategory.SCP_939 => "SCP-939",
            RankCategory.SCP_3114 => "SCP-3114",
            _ => "?"
        };
    }

    public static string GetColor(this RankCategory category)
    {
        return category switch
        {
            RankCategory.D계급 => RoleTypeId.ClassD.GetColor().ToHex(),
            RankCategory.과학자 => RoleTypeId.Scientist.GetColor().ToHex(),
            RankCategory.시설_경비 => RoleTypeId.FacilityGuard.GetColor().ToHex(),
            RankCategory.구미호 => RoleTypeId.NtfPrivate.GetColor().ToHex(),
            RankCategory.반란 => RoleTypeId.ChaosRifleman.GetColor().ToHex(),
            RankCategory.튜토리얼 => RoleTypeId.Tutorial.GetColor().ToHex(),
            RankCategory.SCP_049 => RoleTypeId.Scp049.GetColor().ToHex(),
            RankCategory.SCP_049_2 => RoleTypeId.Scp049.GetColor().ToHex(),
            RankCategory.SCP_079 => RoleTypeId.Scp049.GetColor().ToHex(),
            RankCategory.SCP_096 => RoleTypeId.Scp049.GetColor().ToHex(),
            RankCategory.SCP_106 => RoleTypeId.Scp049.GetColor().ToHex(),
            RankCategory.SCP_173 => RoleTypeId.Scp049.GetColor().ToHex(),
            RankCategory.SCP_939 => RoleTypeId.Scp049.GetColor().ToHex(),
            RankCategory.SCP_3114 => RoleTypeId.Scp049.GetColor().ToHex(),
            _ => "white"
        };
    }
}

public enum RankCategory
{
    D계급,
    과학자,
    시설_경비,
    구미호,
    반란,
    튜토리얼,
    SCP_049,
    SCP_049_2,
    SCP_079,
    SCP_096,
    SCP_106,
    SCP_173,
    SCP_939,
    SCP_3114
}

public enum RankAbilityCategory
{
    변칙성,
    가젯,
    기어
}

public enum RankAbilityType
{
    None,

    // 기어 (공통)
    // D계급
    // 과학자
    // 시설 경비
    // 구미호
    // 반란
    // 튜토리얼

    // SCP-049
    // SCP-049-2
    // SCP-079
    // SCP-096
    // SCP-106
    // SCP-173
    // SCP-939
    // SCP-3114
}

public class RankVar
{
    public static Dictionary<RankAbilityType, RankAbilityData> RankAbilities = new Dictionary<RankAbilityType, RankAbilityData>();
    public static Dictionary<Player, List<RankAbility>> PlayerRankAbilities = new Dictionary<Player, List<RankAbility>>();
}