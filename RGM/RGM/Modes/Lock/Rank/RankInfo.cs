using Exiled.API.Features;
using System.Collections.Generic;

namespace RGM.Modes
{
    public class RankInfo
    {
        public static Dictionary<RankAbilityType, RankAbilityData> RankAbilities = new Dictionary<RankAbilityType, RankAbilityData>();
        public static Dictionary<Player, Dictionary<RankCategory, List<RankAbilityType>>> PlayerRankSettingAbilities = new();
        public static Dictionary<Player, List<RankAbility>> PlayerRankAbilities = new();
        public static Dictionary<Player, bool> PlayerShowRanks = new();

        public static Dictionary<RankCategory, Dictionary<string, (string, RankAbilityType)>> 변칙성 = new()
        {
            { RankCategory.D계급, new()
            {
            {"밤눈", ("야간 투시 효과가 적용됩니다.", RankAbilityType.밤눈)},
            {"강렬한 의지", ("최초 1회에 한하여 부정적인 효과를 제거합니다.", RankAbilityType.강렬한_의지)},
            }
            },
            { RankCategory.과학자, new()
            {
            {"스피드런", ("시설 관리자 키카드를 얻으며, 즉시 고위험군으로 이동합니다.", RankAbilityType.스피드런)},
            {"지문 인식", ("모든 잠긴 문에 엑세스할 수 있으며, 테슬라를 작동시키지 않습니다.", RankAbilityType.지문_인식)},
            }
            },
            { RankCategory.시설_경비, new()
            {
            {"위기 감지", ("주변에 반란이 있다면 시야가 잠시 흐려집니다.", RankAbilityType.위기_감지)},
            {"변칙 감지", ("주변에 SCP가 있다면 시야가 잠시 흐려집니다.", RankAbilityType.변칙_감지)},
            }
            },
            { RankCategory.구미호, new()
            {
            {"베테랑", ("최초 1회에 한하여 사망 판정을 받을 경우 한번 버티며, 1.5초간 무적 효과를 얻습니다.", RankAbilityType.베테랑)},
            {"조직", ("주변에 있는 아군 수만큼 공격력이 2.36% 증가합니다.", RankAbilityType.조직)},
            }
            },
            { RankCategory.반란, new()
            {
            {"보호 장비", ("부정적 특성 효과를 받을 경우 4초 뒤 강제 해제합니다.", RankAbilityType.보호장비)},
            {"송수신 해킹", ("무전기가 지급됩니다.", RankAbilityType.송수신_해킹)},
            }
            },
            { RankCategory.튜토리얼, new()
            {
            {"예능", ("점프력이 20% 향상됩니다.", RankAbilityType.예능)},
            {"선천적 체질", ("바디샷 감소 및 데미지 감소 효과를 15% 얻습니다.", RankAbilityType.선천적_체질)},
            }
            },
            { RankCategory.SCP_049, new()
            {
            {"장의사", ("사망한 지 15초가 된 시체도 되살릴 수 있습니다.", RankAbilityType.장의사)},
            {"사신", ("의사의 감각이 이동 속도를 15% 추가로 올려줍니다.", RankAbilityType.사신)},
            {"자원봉사자", ("의사의 부름이 흄쉴드를 초당 25씩 회복시킵니다.", RankAbilityType.자원봉사자)},
            }
            },
            { RankCategory.SCP_049_2, new()
            {
            {"광기", ("체력이 100 추가되고, SCP-207 효과를 얻습니다. 단, 받는 피해가 25% 증가합니다.", RankAbilityType.광기)},
            {"요리사", ("체력 회복량이 100% 추가됩니다.", RankAbilityType.요리사)},
            }
            },
            { RankCategory.SCP_079, new()
            {
            {"Install", ("10초마다 20씩 경험치를 얻습니다.(최대 6회)", RankAbilityType.Install)},
            {"Upgrade", ("경험치 획득량이 17% 증가합니다.", RankAbilityType.Upgrade)},
            {"Mute", ("SCP-2176에 면역이 됩니다.", RankAbilityType.Mute)},
            }
            },
            { RankCategory.SCP_096, new()
            {
            {"본능", ("분노 시에 30m 내의 인간들을 목격자에 포함시킵니다. (최대 4명)", RankAbilityType.본능)},
            {"날카로움Ⅴ", ("공격력이 90으로 고정됩니다.", RankAbilityType.날카로움_5)},
            }
            },
            { RankCategory.SCP_106, new()
            {
            {"23분 카레", ("체력이 1200이 되는 대신 탄약을 사용하는 총기 관련 데미지에 85% 저항을 가지며, 흄 쉴드가 90% 감소합니다.", RankAbilityType.이십삼분_카레)},
            {"가벼운 주머니", ("한방에 차원 주머니로 보내는 대신, 차원 주머니 탈출 확률이 50%로 조정됩니다.", RankAbilityType.가벼운_주머니)},
            }
            },
            { RankCategory.SCP_173, new()
            {
            {"좋아, 자연스러웠어!", ("순간이동 시, 0.8초 동안 은신 상태가 됩니다.", RankAbilityType.좋아_자연스러웠어)},
            {"피규어", ("체력이 500 증가하고, 몸 크기가 0.8로 조정됩니다.", RankAbilityType.피규어)},
            }
            },
            { RankCategory.SCP_939, new()
            {
            {"암살 조장", ("발걸음 소리가 제거되고 흐리게 보입니다. 이동 속도가 8% 증가합니다.", RankAbilityType.암살_조장)},
            {"발톱 강화", ("발톱 공격의 데미지가 12 증가하고, 공격 시 20 HS를 회복합니다.(최대 1000)", RankAbilityType.발톱강화)},
            }
            },
            { RankCategory.SCP_3114, new()
            {
            {"살점은 나의 힘", ("인간을 공격할 때마다 체력이 15 회복됩니다.", RankAbilityType.살점은_나의_힘)},
            {"단백질", ("변장 해제 후 이동 속도가 10초간 25% 증가합니다.", RankAbilityType.단백질)},
            }
            },
        };

        public static Dictionary<RankCategory, Dictionary<string, (string, RankAbilityType)>> 가젯 = new()
        {
            { RankCategory.D계급, new()
            {
            {"전력 질주", ("5초간 빠르게 이동합니다. 이후 2초간 이동 속도가 감소합니다.", RankAbilityType.전력_질주)},
            {"또수코인", ("코인을 획득합니다. 이 코인을 튕기면 이동 속도가 +3% 누적되지만, 10% 확률로 초기화됩니다.", RankAbilityType.또수코인)},
            }
            },
            { RankCategory.과학자, new()
            {
            {"비브라늄", ("8초간 어떠한 피해도 받지 않는 대신, 4초간 움직일 수 없습니다.", RankAbilityType.비브라늄)},
            {"호신용 후추 스프레이", ("근접해 있는 적에게 일시적으로 부식과 흐릿함, 감속 효과를 부여합니다.", RankAbilityType.호신용_후추_스프레이)},
            }
            },
            { RankCategory.시설_경비, new()
            {
            {"구보", ("8초간 아무런 아이템을 들 수 없는 대신, 이동 속도가 40% 증가합니다. 이후, 1.5초간 움직일 수 없습니다.", RankAbilityType.구보)},
            {"이중 탄창", ("즉시 탄창을 30 장전합니다.", RankAbilityType.이중_탄창)},
            }
            },
            { RankCategory.구미호, new()
            {
            {"몰래 챙겨온 초콜릿", ("속도가 8% 느려지는 대신, 체력이 10초 동안 5씩 회복합니다.", RankAbilityType.몰래_챙겨온_초콜릿)},
            {"용도 외 사용금지", ("8초간 투시 효과를 얻습니다.", RankAbilityType.용도_외_사용금지)},
            }
            },
            { RankCategory.반란, new()
            {
            {"스펀지", ("15초간 발걸음 소리가 사라집니다.", RankAbilityType.스펀지)},
            {"연결 확인", ("반란이 몇 명 살아있는지 확인합니다.", RankAbilityType.연결_확인)},
            }
            },
            { RankCategory.튜토리얼, new()
            {
            {"프로그램", ("10초 동안 점프력이 50% 추가로 향상됩니다.", RankAbilityType.프로그램)},
            {"변칙성 이동기", ("랜덤한 아군에게 순간이동합니다.", RankAbilityType.변칙성_이동기)},
            }
            },
            { RankCategory.SCP_049, new()
            {
            {"걸작", ("다음에 살리는 SCP-049-2의 체력이 50% 추가됩니다.", RankAbilityType.걸작)},
            {"집결", ("SCP-049-2들을 전부 SCP-049의 위치로 이동시킵니다.", RankAbilityType.집결)},
            }
            },
            { RankCategory.SCP_049_2, new()
            {
            {"강펀치", ("스킬 사용 후 다음 공격은 적을 즉사시킵니다.", RankAbilityType.강펀치)},
            {"유대", ("즉시 SCP-049 위치로 이동합니다.", RankAbilityType.유대)},
            }
            },
            { RankCategory.SCP_079, new()
            {
            {"Fix", ("보고 있는 방의 부서진 문 중 하나를 복구합니다.", RankAbilityType.Fix)},
            {"Brake", ("보고 있는 방의 인간들에게 8초간 부식 효과를 적용합니다.", RankAbilityType.Brake)},
            }
            },
            { RankCategory.SCP_096, new()
            {
            {"분노조절문제", ("즉시 분노합니다. 대신 유지 시간이 8초로 조정됩니다.", RankAbilityType.분노조절문제)},
            {"아드레날린", ("8초간 데미지를 50% 적게 받습니다. (분노 중 한정)", RankAbilityType.아드레날린)},
            }
            },
            { RankCategory.SCP_106, new()
            {
            {"비타민 C", ("즉시 기력을 100% 회복합니다.", RankAbilityType.비타민_C)},
            {"참호", ("근처에 있는 문을 5초간 닫고, 잠급니다.", RankAbilityType.참호)},
            }
            },
            { RankCategory.SCP_173, new()
            {
            {"엇박자", ("다음 이동 쿨타임을 즉시 1초로 조정합니다.", RankAbilityType.엇박자)},
            {"부드럽고 따뜻한 호박 고구마", ("즉시 웅덩이를 만듭니다.", RankAbilityType.부드럽고_따뜻한_호박_고구마)},
            }
            },
            { RankCategory.SCP_939, new()
            {
            {"목표를 포착했다", ("15초 동안 시야가 개선되고 스테미나가 무제한이 됩니다.", RankAbilityType.목표를_포착했다)},
            {"유독성 가스", ("안개에 있는 적들에게 4초간 부식 효과를 적용하고 40의 데미지를 입힙니다.", RankAbilityType.유독성_가스)},
            }
            },
            { RankCategory.SCP_3114, new()
            {
            {"트릭", ("변장하지 않은 상태여도 20초간 인간으로 보입니다.", RankAbilityType.트릭)},
            {"외골격", ("흄 쉴드를 모두 소모하여 60% 만큼 체력으로 치환합니다.", RankAbilityType.외골격)},
            }
            },
        };

        public static Dictionary<string, (string, RankAbilityType)> 기어_메인 = new()
        {
            {"공격", ("데미지가 16% 증가합니다.", RankAbilityType.공격)},
            {"방어", ("받는 데미지가 9% 줄어듭니다.", RankAbilityType.방어)},
        };

        public static Dictionary<string, (string, RankAbilityType)> 기어_유틸 = new()
        {
            {"속도", ("이동 속도가 6% 증가합니다.", RankAbilityType.속도)},
            {"치유", ("회복량이 50% 증가합니다.", RankAbilityType.치유)},
            {"효율", ("가젯 재사용 대기 시간이 10% 줄어듭니다.", RankAbilityType.효율)},
        };
    }
}
