using DAONTFT.Core.TFT;
using Exiled.API.Features;
using PlayerRoles;
using RGM.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.Modes
{
    public class RankInfo
    {
        public static Dictionary<RankAbilityType, RankAbilityData> RankAbilities = new Dictionary<RankAbilityType, RankAbilityData>();
        public static Dictionary<Player, Dictionary<RankCategory, List<RankAbilityType>>> PlayerRankSettingAbilities = new();
        public static Dictionary<Player, List<RankAbility>> PlayerRankAbilities = new();

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
            {"선임 연구원", ("구역 관리자 키카드를 얻습니다.", RankAbilityType.선임_연구원)},
            {"지문 인식", ("테슬라를 작동시키지 않습니다.", RankAbilityType.지문_인식)},
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
            {"베테랑", ("최초 1회에 한하여 사망 판정을 받을 경우 한번 버팁니다.", RankAbilityType.베테랑)},
            {"조직", ("주변에 있는 아군 수만큼 공격력이 1% 증가합니다.", RankAbilityType.조직)},
            }
            },
            { RankCategory.반란, new()
            {
            {"방독면", ("부정적인 효과의 세기와 길이를 30% 감소시킵니다.", RankAbilityType.방독면)},
            {"송수신 해킹", ("무전기가 지급됩니다.", RankAbilityType.송수신_해킹)},
            }
            },
            { RankCategory.튜토리얼, new()
            {
            {"예능", ("점프력이 30% 향상됩니다.", RankAbilityType.예능)},
            {"선천적 체질", ("바디샷 감소 효과를 30% 얻습니다.", RankAbilityType.선천적_체질)},
            }
            },
            { RankCategory.SCP_049, new()
            {
            {"장의사", ("사망한 지 15초가 된 시체도 되살릴 수 있습니다.", RankAbilityType.장의사)},
            {"사신", ("의사의 감각이 이동 속도를 10% 추가로 올려줍니다.", RankAbilityType.사신)},
            {"자원봉사자", ("의사의 부름이 흄쉴드를 초당 20씩 회복시킵니다.", RankAbilityType.자원봉사자)},
            }
            },
            { RankCategory.SCP_049_2, new()
            {
            {"광기", ("체력이 100 추가되고, SCP-207 효과를 얻습니다.", RankAbilityType.광기)},
            {"요리사", ("체력 회복량이 100% 추가됩니다.", RankAbilityType.요리사)},
            }
            },
            { RankCategory.SCP_079, new()
            {
            {"Install", ("80초 간 10초마다 10씩 경험치를 얻습니다.", RankAbilityType.Install)},
            {"Upgrade", ("경험치 획득량이 20% 증가합니다.", RankAbilityType.Upgrade)},
            {"Mute", ("SCP-2176에 면역이 됩니다.", RankAbilityType.Mute)},
            }
            },
            { RankCategory.SCP_096, new()
            {
            {"본능", ("주변에 있는 인간들이 점점 위를 올려다보게 됩니다.", RankAbilityType.본능)},
            {"날카로움Ⅴ", ("공격력이 39 증가합니다.", RankAbilityType.날카로움_5)},
            }
            },
            { RankCategory.SCP_106, new()
            {
            {"23분 카레", ("체력이 850이 되는 대신 총기 관련 데미지에 90% 저항을 가지며, 흄 쉴드가 삭제됩니다.", RankAbilityType.이십삼분_카레)},
            {"가벼운 주머니", ("한방에 차원 주머니로 보내는 대신, 차원 주머니 탈출 확률이 50%로 조정됩니다.", RankAbilityType.가벼운_주머니)},
            }
            },
            { RankCategory.SCP_173, new()
            {
            {"좋아, 자연스러웠어!", ("순간이동 시, 0.7초 동안 은신 상태가 됩니다.", RankAbilityType.좋아_자연스러웠어)},
            {"피규어", ("체력이 500 증가하고, 몸 크기가 0.8로 조정됩니다.", RankAbilityType.피규어)},
            }
            },
            { RankCategory.SCP_939, new()
            {
            {"암살 조장", ("발걸음 소리가 줄어들고 흐리게 보입니다.", RankAbilityType.암살_조장)},
            {"흉내쟁이", ("흉내 쿨다운이 0.1초로 조정됩니다.", RankAbilityType.흉내쟁이)},
            }
            },
            { RankCategory.SCP_3114, new()
            {
            {"살점은 나의 힘", ("인간을 처치할 때마다 체력이 50 회복됩니다.", RankAbilityType.살점은_나의_힘)},
            {"단백질", ("변장 해제 후 목 조르기 스킬 사용 기간이 20초로 늘어납니다.", RankAbilityType.단백질)},
            }
            },
        };

        public static Dictionary<RankCategory, Dictionary<string, (string, RankAbilityType)>> 가젯 = new()
        {
            { RankCategory.D계급, new()
            {
            {"전력 질주", ("0.5초간 빠르게 이동합니다.", RankAbilityType.전력_질주)},
            {"또수코인", ("코인을 획득합니다. 이 코인을 튕기면 2% 빨라지거나, 1% 느려집니다.", RankAbilityType.또수코인)},
            }
            },
            { RankCategory.과학자, new()
            {
            {"변칙성 배리어", ("0.5초간 어떠한 피해도 받지 않는 대신, 움직일 수 없습니다.", RankAbilityType.변칙성_배리어)},
            {"변칙성 인식 저해", ("쳐다보는 적들에게 일시적으로 부식과 흐릿함 효과를 부여합니다.", RankAbilityType.변칙성_인식_저해)},
            }
            },
            { RankCategory.시설_경비, new()
            {
            {"구보", ("5초간 아무런 아이템을 들 수 없는 대신, 이동 속도가 12% 증가합니다.", RankAbilityType.구보)},
            {"이중 탄창", ("즉시 탄창을 30 장전합니다. (최대치를 넘길 수 없습니다.)", RankAbilityType.이중_탄창)},
            }
            },
            { RankCategory.구미호, new()
            {
            {"몰래 챙겨온 초콜릿", ("체력이 3초 동안 12씩 회복합니다.", RankAbilityType.몰래_챙겨온_초콜릿)},
            {"용도 외 사용금지", ("4초간 투시 효과를 얻습니다.", RankAbilityType.용도_외_사용금지)},
            }
            },
            { RankCategory.반란, new()
            {
            {"스펀지", ("6초간 발걸음 소리가 사라집니다.", RankAbilityType.스펀지)},
            {"텔레파시", ("아군이 몇 명 살아있는지 확인합니다.", RankAbilityType.텔레파시)},
            }
            },
            { RankCategory.튜토리얼, new()
            {
            {"프로그램", ("10초 동안 점프력이 70% 추가로 향상됩니다.", RankAbilityType.프로그램)},
            {"변칙성 이동기", ("랜덤한 아군에게 순간이동합니다.", RankAbilityType.변칙성_이동기)},
            }
            },
            { RankCategory.SCP_049, new()
            {
            {"걸작", ("다음에 살리는 SCP-049-2의 체력이 100 추가됩니다.", RankAbilityType.걸작)},
            {"집결", ("SCP-049-2들을 전부 SCP-049의 위치로 이동시킵니다.", RankAbilityType.집결)},
            }
            },
            { RankCategory.SCP_049_2, new()
            {
            {"강펀치", ("다음 공격의 데미지가 55 추가됩니다.", RankAbilityType.강펀치)},
            {"유대", ("즉시 SCP-049 위치로 이동합니다.", RankAbilityType.유대)},
            }
            },
            { RankCategory.SCP_079, new()
            {
            {"Fix", ("보고 있는 방의 부서진 문 중 하나를 복구합니다.", RankAbilityType.Fix)},
            {"Brake", ("보고 있는 방의 인간들에게 2초간 부식 효과를 적용합니다.", RankAbilityType.Brake)},
            }
            },
            { RankCategory.SCP_096, new()
            {
            {"허상", ("즉시 분노합니다. 대신 유지 시간이 10초로 조정됩니다.", RankAbilityType.허상)},
            {"아드레날린", ("10초간 데미지를 40% 적게 받습니다. (분노 시 한정)", RankAbilityType.아드레날린)},
            }
            },
            { RankCategory.SCP_106, new()
            {
            {"비타민 C", ("즉시 기력을 20% 회복합니다.", RankAbilityType.비타민_C)},
            {"참호", ("근처에 있는 문을 5초간 닫고, 잠급니다.", RankAbilityType.참호)},
            }
            },
            { RankCategory.SCP_173, new()
            {
            {"엇박자", ("다음 이동 쿨타임을 즉시 1초로 조정합니다.", RankAbilityType.엇박자)},
            {"망원경", ("다음 순간이동 거리를 1.5배 늘립니다.", RankAbilityType.망원경)},
            }
            },
            { RankCategory.SCP_939, new()
            {
            {"목표를 포착했다", ("8초 동안 시야가 개선되고 스테미나가 무제한이 됩니다.", RankAbilityType.목표를_포착했다)},
            {"유독성 가스", ("안개에 있는 적들에게 2초간 부식 효과를 적용하고 20의 데미지를 입힙니다.", RankAbilityType.유독성_가스)},
            }
            },
            { RankCategory.SCP_3114, new()
            {
            {"트릭", ("변장하지 않은 상태여도 10초간 인간으로 보입니다.", RankAbilityType.트릭)},
            {"외골격", ("흄 쉴드를 모두 소모하여 50% 만큼 체력으로 치환합니다.", RankAbilityType.외골격)},
            }
            },
        };

        public static Dictionary<string, (string, RankAbilityType)> 기어 = new()
        {
            {"공격", ("체력이 50% 이하가 되면 입히는 데미지가 10% 증가합니다.", RankAbilityType.공격)},
            {"속도", ("이동 속도가 5% 증가합니다.", RankAbilityType.속도)},
            {"방어", ("받는 데미지가 10% 줄어듭니다.", RankAbilityType.방어)},
            {"치유", ("회복량이 30% 증가합니다.", RankAbilityType.치유)},
            {"효율", ("가젯 재사용 대기 시간이 10% 줄어듭니다.", RankAbilityType.효율)},
        };
    }
}
