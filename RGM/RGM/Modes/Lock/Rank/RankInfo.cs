using PlayerRoles;
using RGM.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.RGM.Modes.Lock.Rank
{
    public class RankInfo
    {
        public static Dictionary<RankCategory, Dictionary<string, (string, RankAbilityType)>> 변칙성 = new()
        {
            { RankCategory.D계급, new()
            {
            {"밤눈", ("야간 투시 효과가 적용됩니다.", RankAbilityType.)},
            {"살고 싶어!", ("피격당하면 이동 속도가 일시적으로 증가합니다.", RankAbilityType.)},
            {"경험자", ("SCP_1853의 효과를 얻습니다.", RankAbilityType.)},
            {"뒷주머니", ("무작위 사탕을 얻습니다.", RankAbilityType.)},
            {"강렬한 의지", ("최초 1회에 한하여 부정적인 효과를 받을 경우 즉시 해제합니다.", RankAbilityType.)},
            }
            },
            { RankCategory.과학자, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.시설_경비, new()
            {
            {"위기 감지", ("주변에 인간 적대 진영이 있다면 시야가 잠시 흐려집니다.", RankAbilityType.)},
            {"희망 감지", ("주변에 아군이 있다면 시야가 잠시 흐려집니다.", RankAbilityType.)},
            {"변칙 감지", ("주변에 SCP가 있다면 시야가 잠시 흐려집니다.", RankAbilityType.)},
            {"선임 경비", ("Crossvec와 추가 섬광탄, 추가 탄약을 얻습니다.", RankAbilityType.)},
            {"숙련자", ("기어 효과가 2배가 됩니다.", RankAbilityType.)},
            }
            },
            { RankCategory.구미호, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.반란, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.튜토리얼, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_049, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_049_2, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_079, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_096, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_106, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_173, new()
            {
            {"좋아, 자연스러웠어!", ("순간이동 시, 0.7초 동안 은신 상태가 됩니다.", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_939, new()
            {
            {"암살 조장", ("발걸음 소리가 줄어듭니다.", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_3114, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
        };

        public static Dictionary<RankCategory, Dictionary<string, (string, RankAbilityType)>> 가젯 = new()
        {
            { RankCategory.D계급, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.과학자, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.시설_경비, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.구미호, new()
            {
            {"몰래 챙겨온 초콜릿", ("체력이 3초 동안 12씩 회복합니다.", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.반란, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.튜토리얼, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_049, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_049_2, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_079, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_096, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_106, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_173, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_939, new()
            {
            {"아드레날린", ("5초 동안 시야가 개선되고 스테미나가 무제한이 됩니다.", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
            { RankCategory.SCP_3114, new()
            {
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            {"", ("", RankAbilityType.)},
            }
            },
        };

        public static Dictionary<string, string> 기어 = new()
        {
            {"공격", "체력이 50% 이하가 되면 입히는 데미지가 10% 증가합니다."},
            {"속도", "이동 속도가 5% 증가합니다."},
            {"방어", "받는 데미지가 10% 줄어듭니다."},
            {"치유", "회복량이 30% 증가합니다."},
            {"효율", "가젯 재사용 대기 시간이 10% 줄어듭니다."},
        };
    }
}
