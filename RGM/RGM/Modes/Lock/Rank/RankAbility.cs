using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Reflection;
using global::UserSettings.ServerSpecific;
using RGM.API.Features;

namespace RGM.Modes;

public abstract class RankAbility
{
    public abstract void OnEnabled();

    public abstract void OnDisabled();

    public RankAbilityData Data { get; set; }
    public Player Owner { get; set; }
}

public abstract class RankGadgetAbility : RankAbility
{
    public bool IsOnCooldown { get; private set; }

    public override void OnEnabled()
    {
        ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSSInput;
    }

    public override void OnDisabled()
    {
        ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSSInput;
    }

    private void OnSSInput(ReferenceHub sender, ServerSpecificSettingBase setting)
    {
        var player = Player.Get(sender);

        if (player != Owner || IsOnCooldown)
            return;

        var gadgetAttr = this.GetType().GetCustomAttribute<RankGadgetAttribute>();
        if (gadgetAttr == null) return;

        if (setting is SSKeybindSetting keybind && keybind.SyncIsPressed && keybind.SettingId == gadgetAttr.SettingId)
        {
            if (!CanUseGadget())
                return;

            OnGadgetUsed();

            Tools.PlaySound(player.Transform, "Gadget");

            IEnumerator<float> onCooldown()
            {
                IsOnCooldown = true;
                string originalDesc = gadgetAttr.Description;
                int cooldown = player.HasRankAbility(RankAbilityType.효율) ? (int)(gadgetAttr.Cooldown * 0.9f) : gadgetAttr.Cooldown;

                for (int i = 0; i < cooldown; i++)
                {
                    Data.Description = $"{originalDesc} ({cooldown - i})";
                    yield return Timing.WaitForSeconds(1);
                }

                Data.Description = originalDesc;
                IsOnCooldown = false;
            }

            Timing.RunCoroutine(onCooldown());
        }
    }

    protected virtual bool CanUseGadget()
    {
        return true;
    }

    protected abstract void OnGadgetUsed();
}

public class RankAbilityData
{
    public Type Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Emoji { get; set; } = "🎲";
    public RankAbilityType RankAbilityType { get; set; }
    public RankCategory RankCategory { get; set; }
    public RankAbilityCategory RankAbilityCategory { get; set; }

    public string GetFormattedName()
    {
        return $"<color={RankAbilityCategory.GetColor()}>{RankAbilityCategory.GetTranslation()}</color> {Emoji} {Name}";
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class RankAbilityAttribute(string name, string description, RankAbilityType type, RankCategory rankCategory, RankAbilityCategory rankAbilityCategory, string emoji = "🎲") : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string Emoji { get; set; } = emoji;
    public RankAbilityType Type { get; set; } = type;
    public RankCategory RankCategory { get; set; } = rankCategory;
    public RankAbilityCategory RankAbilityCategory { get; set; } = rankAbilityCategory;
}

[AttributeUsage(AttributeTargets.Class)]
public class RankGadgetAttribute(string name, string description, RankAbilityType type, RankCategory rankCategory, string emoji = "🎲", int cooldown = 100, int settingId = 289289923) 
    : RankAbilityAttribute(name, description, type, rankCategory, RankAbilityCategory.가젯, emoji)
{
    public int SettingId { get; } = settingId;
    public int Cooldown { get; } = cooldown;
}   

public static class RankAbilityTypeExtensions
{
    public static RankAbilityData GetData(this RankAbilityType type)
    {
        if (!RankInfo.RankAbilities.TryGetValue(type, out var RankAbility))
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
            RankAbilityCategory.기어_메인 => "메인 기어",
            RankAbilityCategory.기어_유틸 => "유틸 기어",
            _ => "?"
        };
    }

    public static string GetColor(this RankAbilityCategory category)
    {
        return category switch
        {
            RankAbilityCategory.변칙성 => "#e4ea4c",
            RankAbilityCategory.가젯 => "#78f168",
            RankAbilityCategory.기어_메인 => "#91edd3",
            RankAbilityCategory.기어_유틸 => "#91edd3",
            _ => "white"
        };
    }
}

public static class RankCategoryExtensions
{
    public static RankCategory GetRankCategory(this Player player)
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

    public static RoleTypeId GetRoleCategory(this RankCategory category)
    {
        return category switch
        {
            RankCategory.D계급 => RoleTypeId.ClassD,
            RankCategory.과학자 => RoleTypeId.Scientist,
            RankCategory.시설_경비 => RoleTypeId.FacilityGuard,
            RankCategory.구미호 => RoleTypeId.NtfPrivate,
            RankCategory.반란 => RoleTypeId.ChaosRifleman,
            RankCategory.튜토리얼 => RoleTypeId.Tutorial,
            RankCategory.SCP_049 => RoleTypeId.Scp049,
            RankCategory.SCP_049_2 => RoleTypeId.Scp0492,
            RankCategory.SCP_079 => RoleTypeId.Scp079,
            RankCategory.SCP_096 => RoleTypeId.Scp096,
            RankCategory.SCP_106 => RoleTypeId.Scp106,
            RankCategory.SCP_173 => RoleTypeId.Scp173,
            RankCategory.SCP_939 => RoleTypeId.Scp939,
            RankCategory.SCP_3114 => RoleTypeId.Scp3114,
            _ => RoleTypeId.Tutorial
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
    공통,
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
    기어_메인,
    기어_유틸
}

public enum RankAbilityType
{
    None,

    // 기어 (공통)
    공격,
    속도,
    방어,
    치유,
    효율,

    // 변칙성
    // D계급
    밤눈,
    강렬한_의지,

    // 과학자
    스피드런,
    지문_인식,

    // 시설 경비
    위기_감지,
    변칙_감지,

    // 구미호
    베테랑,
    조직,

    // 반란
    보호장비,
    송수신_해킹,

    // 튜토리얼
    예능,
    선천적_체질,

    // SCP-049
    장의사,
    사신,
    자원봉사자,

    // SCP-049-2
    광기,
    요리사,

    // SCP-079
    Install,
    Upgrade,
    Mute,

    // SCP-096
    본능,
    날카로움_5,

    // SCP-106
    이십삼분_카레,
    가벼운_주머니,

    // SCP-173
    좋아_자연스러웠어,
    피규어,

    // SCP-939
    암살_조장,
    발톱강화,

    // SCP-3114
    살점은_나의_힘,
    단백질,

    // 가젯
    // D계급
    전력_질주,
    또수코인,

    // 과학자
    비브라늄,
    호신용_후추_스프레이,

    // 시설 경비
    구보,
    이중_탄창,

    // 구미호
    몰래_챙겨온_초콜릿,
    용도_외_사용금지,

    // 반란
    스펀지,
    연결_확인,

    // 튜토리얼
    프로그램,
    변칙성_이동기,

    // SCP-049
    걸작,
    집결,

    // SCP-049-2
    강펀치,
    유대,

    // SCP-079
    Fix,
    Brake,

    // SCP-096
    분노조절문제,
    아드레날린,

    // SCP-106
    비타민_C,
    참호,

    // SCP-173
    엇박자,
    부드럽고_따뜻한_호박_고구마,

    // SCP-939
    목표를_포착했다,
    유독성_가스,

    // SCP-3114
    트릭,
    외골격,
}

