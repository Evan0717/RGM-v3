using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using RGM.API.Features;

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
    public AbilityHolidayType HolidayType { get; set; }
    public List<AbilityType> Requires { get; set; }
    public bool Keep { get; set; }
    public bool _79Allowed { get; set; }
    public RoleAbility RoleAbility { get; set; }

    public string GetFormattedName()
    {
        string CategoryName = Category.GetCategoryTranslation();
        string Text = "전용";
        return $"<color={Category.GetColor()}>[{(RoleAbility == RoleAbility.None ? CategoryName : $"{Text} {CategoryName}")}]</color> {Name}";
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class AbilityAttribute(string name, string description, AbilityCategory category, AbilityType type, RoleAbility roleAbility = RoleAbility.None, bool _79Allowed = false, AbilityHolidayType holidayType = AbilityHolidayType.None, bool keep = false) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public AbilityCategory Category { get; } = category;
    public AbilityType Type { get; set; } = type;
    public bool _79Allowed { get; } = _79Allowed;
    public RoleAbility RoleAbility { get; } = roleAbility;
    public AbilityHolidayType HolidayType { get; set; } = holidayType;
    public bool Keep { get; set; } = false;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequiresAbilityAttribute(params AbilityType[] abilities) : Attribute
{
    public AbilityType[] Abilities { get; } = abilities;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ConditionAbilityAttribute(AbilityType[] abilities, AbilityType[] blocks) : Attribute
{
    public AbilityType[] Abilities { get; } = abilities;
    public AbilityType[] BlockAbilities { get; } = blocks;
}

public enum AbilityCategory
{
    None,
    Dummy,
    Common,
    Rare,
    Epic,
    Legend,
    Mythic,
    Synergy,
}

public enum RoleAbility
{
    None,
    ClassD,
    Scientist,
    NTF,
    CHI,
    Tutorial,
    Scp173,
    Scp049,
    Scp0492,
    Scp096,
    Scp106,
    Scp939,
    Scp3114,
    Scp079,
    Flamingo
}

/* 

전용 능력에 등급 추가 구상

위의 Enum사용
AbilityData에 bool 79Allowed, RoleAbility RoleAbility 추가
물론 Attribute에도
79Allowed는 럭키비키 등이 포함될 것입니다.

등급에 따른 전용 능력의 확률
일반 ->5퍼
희귀 ->7퍼
영웅 ->10퍼
전설 -> 20퍼
신화 -> 25퍼
위 확률로 선택지들 중 그 선택지를 등급에 맞는 전용 능력으로 바꿉니다.

바꿔야 될게 많은데,

Ability.cs
1. GetAbilityCategory
2. GetTranslation
3. GetColor
4. Enum AbilityType -> 전용등급의 이름만 변경

ABattle.cs
1. RatingColor
2. ColorFormat
3. OnEnabled()
4. StartSelect() -> 제일 중요 ***
5. SelectionCoroutine 내부 로컬함수 -> 2번째로 중요 **
6. GetCategory() 3번째로 중요 *
7. ApplyPrelude()

이거 말고 더 있을 수 있습니다 제가 찾은 거는.
일단 AbilityCategory 쓰는 거는 다 잡아야 돼요.

 */

public static class AbilityCategoryExtensions
{
    public static string GetCategoryTranslation(this AbilityCategory category)
    {
        return category switch
        {
            AbilityCategory.Common => "일반",
            AbilityCategory.Rare => "희귀",
            AbilityCategory.Epic => "영웅",
            AbilityCategory.Legend => "전설",
            AbilityCategory.Mythic => "신화",
            AbilityCategory.Synergy => "시너지",
            _ => "?"
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
            AbilityCategory.Synergy => "#DEEFED",
            _ => "white"
        };
    }
}

public static class RoleAbilityExtensions
{
    public static RoleAbility GetRoleAbility(this Player player)
    {
        RoleTypeId role = player.Role.Type;
        return role switch
        {
            RoleTypeId.ClassD => RoleAbility.ClassD,
            RoleTypeId.Scientist => RoleAbility.Scientist,
            RoleTypeId.FacilityGuard => RoleAbility.NTF,
            RoleTypeId.NtfPrivate => RoleAbility.NTF,
            RoleTypeId.NtfSergeant => RoleAbility.NTF,
            RoleTypeId.NtfCaptain => RoleAbility.NTF,
            RoleTypeId.NtfSpecialist => RoleAbility.NTF,
            RoleTypeId.ChaosRifleman => RoleAbility.CHI,
            RoleTypeId.ChaosMarauder => RoleAbility.CHI,
            RoleTypeId.ChaosRepressor => RoleAbility.CHI,
            RoleTypeId.ChaosConscript => RoleAbility.CHI,
            RoleTypeId.Scp173 => RoleAbility.Scp173,
            RoleTypeId.Scp049 => RoleAbility.Scp049,
            RoleTypeId.Scp0492 => RoleAbility.Scp0492,
            RoleTypeId.Scp096 => RoleAbility.Scp096,
            RoleTypeId.Scp106 => RoleAbility.Scp106,
            RoleTypeId.Scp939 => RoleAbility.Scp939,
            RoleTypeId.Scp3114 => RoleAbility.Scp3114,
            RoleTypeId.Scp079 => RoleAbility.Scp079,
            RoleTypeId.Flamingo => RoleAbility.Flamingo,
            RoleTypeId.AlphaFlamingo => RoleAbility.Flamingo,
            RoleTypeId.ChaosFlamingo => RoleAbility.Flamingo,
            RoleTypeId.NtfFlamingo => RoleAbility.Flamingo,
            RoleTypeId.ZombieFlamingo => RoleAbility.Flamingo,
            _ => RoleAbility.None
        };
    }
}

public static class PlayerExtensions 
{
    public static int AbilityCount(this Player player, AbilityType abilityType)
    {
        return ABattle.Instance.PlayerAbilities[player].Count(x => x.Data.AbilityType == abilityType);
    }
}

public enum AbilityHolidayType
{
    None,
    Christmas,
    Halloween,
}

public enum AbilityType
{
    NONE,

    // 더미 //
    DUMMY_EXPIREDINSURANCE, // [더미] 만료된 보험
    DUMMY_DOPAMINERELEASED, // [더미] 방출된 도파민
    DUMMY_TESTSUCCESS, // [더미] 시험 성공
    DUMMY_TESTFAILURE, // [더미] 시험 실패
    DUMMY_USEDADHESIVEPLASTER, // [더미] 해진 반창고
    DUMMY_RARETRANSITIONSUCCESS, // [더미] 하급 변이 성공
    DUMMY_RARETRANSITIONFAILURE, // [더미] 하급 변이 실패
    DUMMY_EPICTRANSITIONSUCCESS, // [더미] 변이 성공
    DUMMY_EPICTRANSITIONFAILURE, // [더미] 변이 실패
    DUMMY_LEGENDTRANSITIONSUCCESS, // [더미] 상급 변이 성공
    DUMMY_LEGENDTRANSITIONFAILURE, // [더미] 상급 변이 실패
    DUMMY_USEDPINGHOOK, // [더미] 핑 갈?고리
    DUMMY_BACKEDUP, // [더미] 백업됨
    DUMMY_ENDOFGRAVEROBBERY, // [더미] 도굴 끝!
    DUMMY_COLDSTEW, // [더미] 식은 스튜
    DUMMY_NOAFK, // [더미] 자리 다비움
    DUMMY_USEDSNAKEHANDRADIO, // [더미] 뱀의 손 수장
    DUMMY_TELEPORTED, // [더미] 순간이동됨
    DUMMY_FINALEXAMSUCCESS, // [더미] 기말고사 수석
    DUMMY_FINALEXAMFAIL, // [더미] 기말고사 낙제
    DUMMY_CSTCSUCCESS, // [더미] 대학수학능력시험 1등급
    DUMMY_CSTCFAIL, // [더미] 대학수학능력시험 9등급

    // 일반 //
    NORMAL_WORKOUT, // [일반] 운동
    NORMAL_SWIFT, // [일반] 경공
    NORMAL_EVOLUTION, // [일반] 진화
    NORMAL_TRAINING, // [일반] 단련
    NORMAL_LUCKY, // [일반] 행운
    NORMAL_STAMINAREPLENISHMENT, // [일반] 체력 보충
    NORMAL_RANDOMBOX, // [일반] 랜덤박스
    NORMAL_FINDLOCATION, // [일반] 위치 추적
    NORMAL_INSURANCE, // [일반] 보험
    NORMAL_KICK, // [일반] 회축
    NORMAL_SUPPLY, // [일반] 보급
    NORMAL_PURIFICATION, // [일반] 정화
    NORMAL_TORCH, // [일반] 횃불
    NORMAL_SNEAK, // [일반] 잠행
    NORMAL_ESCAPE, // [일반] 위기 탈출
    NORMAL_FRIENDSHIP, // [일반] 우애
    NORMAL_RAINBOW, // [일반] 무지개
    NORMAL_BODYBACK, // [일반] 바디백
    NORMAL_DOPAMINE, // [일반] 도파민
    NORMAL_TEST, // [일반] 시험
    NORMAL_AGILITY, // [일반] 민첩
    NORMAL_BULLETSUPPLY, // [일반] 잔탄 수급
    NORMAL_REROLL, // [일반] 리롤
    NORMAL_SUSPICIOUSSTEW, // [일반] 수상한 스튜
    NORMAL_HEALGUN, // [일반] 치유 사제
    NORMAL_MILK, // [일반] 우유
    NORMAL_RUSH, // [일반] 황소
    NORMAL_EXCHANGE, // [일반] 교환
    NORMAL_RABBIT, // [일반] 토끼뜀
    NORMAL_RANDOMCANDY, // [일반] 트릭 오어 트릿

    // 희귀 //
    RARE_PHYSICALSTRENGTHENING, // [희귀] 육체 강화
    RARE_STEELSHELL, // [희귀] 강철 껍질
    RARE_TRANSPARENTCLOAK, // [희귀] 투명 망토
    RARE_VAMPIRE, // [희귀] 흡혈귀
    RARE_TELEPORTATION, // [희귀] 순간이동
    RARE_BOMBERMAN, // [희귀] 봄버맨
    RARE_GRAPPLINGHOOK, // [희귀] 갈고리
    RARE_STOPWATCH, // [희귀] 회중시계
    RARE_STEROID, // [희귀] 스테로이드
    RARE_MARTYRDOM, // [희귀] 순교
    RARE_HYPASS, // [희귀] 하이패스
    RARE_TRIPLEAXEL, // [희귀] 트리플악셀
    RARE_ALCHEMY, // [희귀] 연금
    RARE_ADHESIVEPLASTER, // [희귀] 반창고
    RARE_WEAPONEXPERT, // [희귀] 무기 전문가
    RARE_PANACEA, // [희귀] 만병통치약
    RARE_SALAMANDRA, // [희귀] 불의 정령, 살라만드라
    RARE_UNDINE, // [희귀] 물의 정령, 운디네
    RARE_GNOME, // [희귀] 흙의 정령, 노움
    RARE_SYLPH, // [희귀] 바람의 정령, 실프
    RARE_CONTRACT, // [희귀] 계약
    RARE_AMBUSH, // [희귀] 매복자
    RARE_TRANSITION, // [희귀] 하급 변이
    RARE_UPGRADE, // [희귀] 강화
    RARE_DND, // [희귀] 자리 비움
    RARE_CLONE, // [희귀] 분신
    RARE_SPACETRAVEL, // [희귀] 공간이동
    RARE_ORGANICMILK, // [희귀] 유기농 우유
    RARE_CANDYBAG, // [희귀] 사탕 봉지
    RARE_DOBBYISFREE, // [희귀] 도비는 자유에요
    RARE_FINALEXAM, // [희귀] 기말고사

    // 영웅 //
    EPIC_TERRORISTREMAINS, // [영웅] 테러리스트의 유품
    EPIC_GAMBLER, // [영웅] 도박꾼
    EPIC_RANDOMCHEST, // [영웅] 랜덤상자
    EPIC_REPAIRMAN, // [영웅] 수리 기사
    EPIC_SUPERSTAR, // [영웅] 슈퍼 스타
    EPIC_LUCKYVIKEY, // [영웅] 럭키비키
    EPIC_EXTREMEPOISON, // [영웅] 극독
    EPIC_SURVIVOR, // [영웅] 구사일생
    EPIC_GHOSTRULE, // [영웅] 고스트룰
    EPIC_DIVER, // [영웅] 잠수부
    EPIC_BLINK, // [영웅] 점멸
    EPIC_TRANSITION, // [영웅] 변이
    EPIC_SUICIDEBOMBER, // [영웅] 수어사이드 봄버맨
    EPIC_DISGUISE, // [영웅] 위장술
    EPIC_GRAVEROBBER, // [영웅] 도굴꾼
    EPIC_INFINITYGUN, // [영웅] 훌륭한 대화수단
    EPIC_SCP1344, // [영웅] 투시
    EPIC_PRIEST, // [영웅] 성직자
    EPIC_ADDWORKSTATION, // [영웅] 업무 증가
    EPIC_MADSCIENTIST, // [영웅] 매드 사이언티스트
    EPIC_SCP127, // [영웅] 인생의 동반자
    EPIC_ANTISCP207, // [영웅] 초재생
    EPIC_FOODRESEARCHER, // [영웅] 요리 연구가,
    EPIC_SCP1509, // [영웅] 마체테
    EPIC_MARSHMELLOW, // [영웅] !!마쉬멜로우!!
    EPIC_CONTEXPERT, // [영웅] 격리 전문가
    EPIC_BULLSEYE, // [영웅] 불스아이
    EPIC_RAMBO, // [영웅] 람보
    EPIC_SPRINGFIELDM1A, // [영웅] Springfield M1A
    EPIC_CSTC, // [영웅] 대학수학능력시험
    EPIC_HOLYPROTECTION, // [영웅] 신성방어
    EPIC_AN94, // // [영웅] AN-94

    // 전설 //
    LEGEND_SPEEDWAGON, // [전설] 스피드왜건
    LEGEND_SNAKEHANDRADIO, // [전설] 뱀의 손 무전기
    LEGEND_RANDOMPACKAGE, // [전설] 랜덤택배
    LEGEND_MAGICIAN, // [전설] 마술사
    LEGEND_FLASHLIGHT, // [전설] 플래시라이트
    LEGEND_KILLSTREAK, // [전설] 킬스트릭
    LEGEND_PSYCHICS, // [전설] 영매
    LEGEND_SCREAM, // [전설] 괴성
    LEGEND_TRANSITION, // [전설] 상급 변이
    LEGEND_CANDYADDICT, // [전설] 마약 중독자
    LEGEND_REFLECTOR, // [전설] 반사경
    LEGEND_CATACLYSMGENERATOR, // [전설] 대격변 생성기
    LEGEND_LAVACHICKEN, // [전설] La-La-La Lava Ch-Ch-Ch Chicken
    LEGEND_FLAMETHROWER, // [전설] 화염 방사기
    LEGEND_ASYNC, // [전설] A-Sync Research
    LEGEND_AIMHACK, // [전설] 솔져: 76
    LEGEND_SCP008, // [전설] SCP-008, 좀비 전염병
    LEGEND_SCP035, // [전설] SCP-035, 빙의 가면
    LEGEND_SCP294, // [전설] SCP-294, 커피 자판기
    LEGEND_SCP457, // [전설] SCP-457, 불타는 남자
    LEGEND_SCP966, // [전설] SCP-966, 잠을 죽이는 자
    LEGEND_SCP999, // [전설] SCP-999, 간지럼 괴물
    LEGEND_CANDYPOWER, // [전설] 섬뜩한 힘
    LEGEND_JOHNWICK, // [전설] 존 윅
    LEGEND_TURTLE, // [전설] 거북 도사

    // 신화 //
    MYTHIC_ROCKETLAUNCHER, // [신화] 로켓 런처
    MYTHIC_SPIRIT, // [신화] 스피릿
    MYTHIC_EYEMAN, // [신화] 눈빛맨
    MYTHIC_DIMENSIONTHIEF, // [신화] 차원 강탈자
    MYTHIC_JOKER, // [신화] 조커
    MYTHIC_NOCLIP, // [신화] 노클립
    MYTHIC_BOMBGUN, // [신화] 워 머신
    MYTHIC_WARGOD, // [신화] 광전사
    MYTHIC_BALLISTAEM3, // [신화] 발리스타 MP3
    MYTHIC_TOOLGUN, // [신화] 툴건
    MYTHIC_KINGSCOLOR, // [신화] 패왕색 패기
    MYTHIC_ROSEHIP, // [신화] 장미칼
    MYTHIC_HAMMER, // [신화] 철퇴
    MYTHIC_UNLIMITED, // [신화] 무제한
    MYTHIC_ANCHOR, //[신화] 구속


    // 전용 //
    // D계급
    COMMON_CLASSD_LARCENY, // [전용 일반] 절도죄
    COMMON_CLASSD_SEEDSOFCHI, // [전용 일반] 반란의 씨앗
    COMMON_CLASSD_TRESPASSING, // [전용 희귀] 주거침입죄
    COMMON_CLASSD_ILLEGALWEAPON, // [전용 희귀] 불법개조무기소지죄

    // 과학자
    COMMON_SCIENTIST_ENGINEERINGMAJOR, // [전용 일반] 공학 전공
    COMMON_SCIENTIST_SEEDSOFMTF, // [전용 일반] 특무부대의 씨앗
    COMMON_SCIENTIST_05, // [전용 희귀] 05 평의회

    // NTF
    COMMON_NTF_HEALTHCENTERSTAFF, // [전용 일반] 보건소 직원
    COMMON_NTF_QUARANTINEOBLIGATION, // [전용 일반] 격리 의무자
    COMMON_NTF_COLLECTIVEINTELLIGENCE, // [전용 일반] 집단 지성
    COMMON_NTF_MANAGERIALOBLIGATIONPERSON, // [전용 희귀] 관리 의무자
    COMMON_NTF_INDUSTRIALACCIDENTINSURANCE, // [전용 희귀] 산업재해보험
    COMMON_NTF_MEDICALOFFICER, // [전용 희귀] 의무병
    COMMON_NTF_RADAR, // [전용 희귀] 레이더

    // 혼돈의 반란
    COMMON_CHI_TOUCHOFCHAOS, // [전용 일반] 혼돈의 손길
    COMMON_CHI_BAGOFCHAOS, // [전용 일반] 혼돈의 가방
    COMMON_CHI_CHAOSOFCHAOS, // [전용 희귀] 혼돈의 카오스

    // 뱀의 손
    COMMON_TUTORIAL_TONGUE, // [전용 일반] 세치 혀
    COMMON_TUTORIAL_THIRDFORCE, // [전용 일반] 제3세력
    COMMON_TUTORIAL_RESEARCHER, // [전용 희귀] SCP 연구자

    // SCP-173
    COMMON_SCP173_FEAR, // [전용 일반] 공포
    COMMON_SCP173_ABERRATION, // [전용 희귀] 괴이
    COMMON_SCP173_MIRAGE, // [전용 희귀] 신기루

    // SCP-049
    COMMON_SCP049_DEATH, // [전용 희귀] 사신
    COMMON_SCP049_COMPETENTDOCTOR, // [전용 희귀] 유능한 의사
    COMMON_SCP049_PROFICIENCY, // [전용 희귀] 능수능란
    COMMON_SCP049_MADDOCTOR, // [전용 희귀] 실험체

    // SCP-0492
    COMMON_SCP0492_MEALS, // [전용 일반] 급식
    COMMON_SCP0492_CONFUSION, // [전용 일반] 당혹감
    COMMON_SCP0492_INFECTION, // [전용 일반] 감염
    COMMON_SCP0492_HUNGER, // [전용 희귀] 허기

    // SCP-096
    COMMON_SCP096_ENEMY, // [전용 일반] 원수
    COMMON_SCP096_CANTMANAGEANGER, // [전용 일반] 분노 조절 문제
    COMMON_SCP096_OUTSIDER, // [전용 일반] 아웃사이더
    COMMON_SCP096_RAGE, // [전용 희귀] 격노
    COMMON_SCP096_STARTEARING, // [전용 영웅] 별자리 찢기
    COMMON_SCP096_SEER, // [전용 영웅] 천리안

    // SCP-106
    COMMON_SCP106_RECOVERY, // [전용 희귀] 회춘
    COMMON_SCP106_HUNTINGPREY, // [전용 희귀] 사냥감 모색
    COMMON_SCP106_STICKYSWAMP, // [전용 영웅] 끈적한 늪

    // SCP-939
    COMMON_SCP939_HUGME, // [전용 일반] 그 시절 댕댕이
    COMMON_SCP939_NOEYES, // [전용 일반] 실명
    COMMON_SCP939_REINFORCECLAW, // [전용 희귀] 발톱 강화
    COMMON_SCP939_VAMPIRECLAW, // [전용 희귀] 흡혈 발톱

    // SCP-3114
    COMMON_SCP3114_HALFBLOCK, // [전용 일반] 반블럭
    COMMON_SCP3114_SKILLEDASSASSIN, // [전용 희귀] 숙련된 암살자
    COMMON_SCP3114_DORAEMONPOCKET, // [전용 희귀] 도라에몽 주머니
    COMMON_SCP3114_SHOWMANSHIP, // [전용 희귀] 쇼맨쉽

    // SCP-079
    COMMON_SCP079_PINGREMOTE, // [전용 일반] 핑 리모컨
    COMMON_SCP079_PORTABLECHARGER, // [전용 일반] 간이 충전기
    COMMON_SCP079_RANDOMFUNCTION, // [전용 일반] 랜덤 함수
    COMMON_SCP079_SHUTDOWN, // [전용 일반] 셧다운제
    COMMON_SCP079_OVERCLOCKING, // [전용 일반] 오버클럭
    COMMON_SCP079_JUSTPRICE, // [전용 일반] 응당한 대가
    COMMON_SCP079_CAMERAFLASH, // [전용 일반] 카메라 플래시
    COMMON_SCP079_CASSIE, // [전용 일반] C.A.S.S.I.E.
    COMMON_SCP079_AUTOTESLA, // [전용 일반] 자동 방어 시스템
    RARE_SCP079_OVERCURRENT, // [전용 희귀] 과전류
    RARE_SCP079_OVERWHELMING, // [전용 희귀] 고대의 존재 압도
    RARE_SCP079_POWERABSORPTION, // [전용 희귀] 전력 흡수
    RARE_SCP079_PINGHOOK, // [전용 희귀] 핑 갈고리
    RARE_SCP079_LOCKDOWN, // [전용 희귀] 봉쇄
    RARE_SCP079_REPAIR, // [전용 희귀] 수리수리 마수리
    RARE_SCP079_RESTAREA, // [전용 희귀] 휴게소
    RARE_SCP079_FREEDOM, // [전용 희귀] 자유
    RARE_SCP079_MOBILESTRIKEFORCE, // [전용 희귀] 기동타격대
    RARE_SCP079_AIRSTRIKE, // [전용 희귀] 폭격
    RARE_SCP079_SYSTEMHACKING, // [전용 희귀] 시스템 해킹
    EPIC_SCP079_CALLSCP, // [전용 영웅] SCP 지원 호출기
    LEGEND_SCP079_STARTWARHEAD, // [전용 전설] 자폭 시퀸스
    MYTHIC_SCP079_TOOLPING, // [전용 신화] 따아알깍

    // 플라밍고
    COMMON_FLAMINGO_MINIFACTORY, // [전용 일반] 미니 공장

    // 시너지 //
    SYNERGY_SURVIVALEXPERT, // [시너지] 생존 전문가
    SYNERGY_GLORY, // [시너지] 광휘
    SYNERGY_RANDOMCOLLECTION, // [시너지] 랜덤 컬렉션
    SYNERGY_FOURMAJOREXERCISES, // [시너지] 4대 운동
    SYNERGY_DUPLICATEFATE, // [시너지] 중복 기연
    SYNERGY_DRUID, // [시너지] 드루이드
    SYNERGY_SUICIDESQUAD, // [시너지] 수어사이드 스쿼드
    SYNERGY_ASSASSIN, // [시너지] 암살자
    SYNERGY_LOSER, // [시너지] 패배자
    SYNERGY_WINNER, // [시너지] 승리자
    SYNERGY_VAMPIRE, // [시너지] 뱀파이어,
    SYNERGY_GMAN, // [시너지] G맨
    SYNERGY_RICH1, // [시너지] 부자Ⅰ
    SYNERGY_RICH2, // [시너지] 부자Ⅱ
    SYNERGY_AFK, // [시너지] AFK
    SYNERGY_BOMBPARTY, // [시너지] 폭탄 파티
    SYNERGY_BLACKMARKET, // [시너지] 암시장
}

public static class AbilityTypeExtensions
{
    public static string GetTranslation(this AbilityType type)
    {
        var aBattle = ABattle.Instance;

        if (!aBattle.Abilities.TryGetValue(type, out var ability))
            return "?";

        return ability.GetFormattedName();
    }
}
