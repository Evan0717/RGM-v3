using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM
{
    public class ModeManager
    {
        public static Dictionary<string, List<string>> Modes { get; set; } = new Dictionary<string, List<string>>()
        {
            { "워크스테이션 업그레이드", new List<string>() { "00FFFF", "워크스테이션에서 업그레이드하세요!", "ABattle", "public", "",
"""
<color=#F5DA81>인간 진영</color>일 경우, 워크스테이션에서 점프하면 50% 확률로 능력을 1개 얻습니다.
<color=red>SCP-079</color>일 경우, 같은 팀인 SCP가 능력을 얻으면 10% 확률로 획득합니다.

각 능력 등급들의 확률을 확인하려면 아래를 참고하십시오.
• <color=#A4A4A4>일반</color> - 65%
• <color=#2ECCFA>희귀</color> - 27%
• <color=#FF00FF>영웅</color> - 7%
• <color=#ffd700>전설</color> - 0.4%
• <color=#DF0101>신화</color> - 0.1%
• <color=#F7819F>전용</color> - 100%

20% 확률로 피버 모드가 활성화됩니다.
<b><i><color=#FF00EA>피</color><color=#EF00EB>버</color> <color=#CF00ED>모</color><color=#BF00EF>드</color></i></b> 활성화 시, 재단에 등장하는 워크스테이션의 수가 증가합니다.
""" } },
            { "블랙아웃", new List<string>() { "2A0A0A", "시설 곳곳이 정전됩니다.", "Blackout", "public", "", 
"""
각 방마다 기믹이 적용될 수 있습니다.

33% - 방이 정전됩니다.
33% - 방의 색상이 변경됩니다.
33% - 아무 일도 일어나지 않습니다.
(소숫점 버림)
""" } },
            { "축복", new List<string>() { "F6D8CE", "관전자의 수에 비례해 능력치가 상승합니다.", "Blessing", "public", "", "" } },
            { "폭탄 파티", new List<string>() { "FAAC58", "버티면 버틸수록 난이도가 올라갑니다.", "BombParty", "public", "",
"""
투사체 또는 SCP 아이템이 랜덤한 곳에 떨어집니다.

3초마다 이벤트가 일어납니다.

라운드 경과
0초 후ㅣ<b>고폭 수류탄</b>이 떨어집니다.
30초 후ㅣ<b>섬광탄</b>이 1/3 확률로 떨어집니다.
60초 후ㅣ<b>고폭 수류탄</b>이 1/3 확률로 떨어집니다.
90초 후ㅣ<b>SCP-244</b>가 1/3 확률로 떨어집니다.
120초 후ㅣ<b>SCP-018</b>이 1/3 확률로 떨어집니다.

3분 이상 버틴다면 스스로를 칭찬해주세요.
""" } },
            { "수집가", new List<string>() { "FFBF00", "SCP 아이템 3개를 가지고 시작합니다.", "Collector", "public", "", "" } },
            { "데스런", new List<string>() { "FF4000", "과학자는 죄수들의 접근을 막아야 합니다.\n널리 퍼져 있는 함정들을 조심하십시오!", "DeathRun", "public", "",
"""
<color=yellow>과학자</color>의 경우, <color=red>빨간 버튼</color>을 눌러 함정을 발동시킬 수 있습니다.
단, 함정은 1번만 작동시킬 수 있으므로, 한번에 최대한 많은 <color=orange>D계급</color>들을 죽이세요.
모든 <color=orange>D계급</color>을 죽이거나, 180초를 버티면 <color=yellow>과학자</color>의 승리입니다.

<color=orange>D계급</color>의 경우, 목적지까지 빠르게 도달해야 합니다.
목적지에 도달한 경우 <color=yellow>과학자</color>를 사살하여 승리할 수 있게 됩니다. 총을 얻게 되는 거죠!
""" } },
            { "사회적 거리두기", new List<string>() { "38610B", "최대한 다른 사람과 멀어지세요! 감염 예방이 최우선입니다!", "Distancing", "private", "", "" } },
            { "더블업", new List<string>() { "5882FA", "모드 2개가 동시에 적용됩니다.", "DoubleUp", "public", "", "" } },
            { "개인전", new List<string>() { "FA58F4", "최후의 1인이 되세요!", "FreeForAll", "public", "", "" } },
            { "어제의 동지는 오늘의 적", new List<string>() { "F78181", "팀킬이 가능합니다, 이젠 아무도 믿지 마세요.\nSCP도 마찬가지입니다!", "FriendlyFire", "public", "", "" } },
            { "도박", new List<string>() { "8A4B08", "아이템을 떨구면 새로운 아이템을 획득합니다.\n단, 2% 확률로 손이 잘립니다.", "Gamble", "public", "", "" } },
            { "기브 어웨이", new List<string>() { "E6E6E6", "60초마다 랜덤한 아이템이 지급됩니다.", "Giveaway", "private", "", "" } },
            { "Gun Game", new List<string>() { "088A08", "대인전에 자신 있으신가요?\n실력을 증명하기 위해 우승을 차지하세요!", "GunGame", "public", "", "" } },
            { "HIDE", new List<string>() { "0489B1", "숨 죽이는 그를 사살하십시오.", "HIDE", "public", "", "" } },
            { "GG 클럽", new List<string>() { "C8FE2E", "빠르게 황금색 플랫폼을 사수하세요!", "GGClub", "public", "", "" } },
            { "폭탄 돌리기", new List<string>() { "FA58D0", "폭탄이 터지기 전에 다른 유저에게 넘기세요!", "HotPotato", "private", "", "" } },
            { "무한 탄약", new List<string>() { "6E6E6E", "무제한 탄약과 함께 승전보를 울리세요!", "InfiniteAmmo", "private", "", "" } },
            { "츄파츕스", new List<string>() { "A9E2F3", "모두가 제일버드를 가지고 시작합니다.", "Jailbird", "public", "", "" } },
            { "존 윅", new List<string>() { "2EFEF7", "권총류 무기의 데미지가 400% 상승합니다.", "JohnWick", "public", "", "" } },
            { "저거너트", new List<string>() { "088A08", "모두 힘을 합쳐 외부의 적에 대항하세요.\n10분에 자동핵이 작동됩니다.", "Juggernaut", "private", "", "" } },
            { "점프맵 라운지", new List<string>() { "A9D0F5", "5분 안에 최대한 멀리 가세요!", "JumpMap", "private", "", "" } },
            { "한국인이 좋아하는 속도", new List<string>() { "5882FA", "이런 거 좋아하시죠?", "KoreanSpeed", "public", "", "" } },
            { "리더", new List<string>() { "64FE2E", "각 진영마다 리더가 정해집니다.\n리더를 도와 진영을 승리로 이끄세요!", "Leader", "private", "", "" } },
            { "미니 게임", new List<string>() { "6E6E6E", "미니 게임 중 하나가 랜덤으로 선택됩니다.\n총 3개의 라운드로 진행됩니다.", "MiniGames", "public", "", "" } },
            { "중첩", new List<string>() { "04B45F", "HP 제한이 제거됩니다.", "Nesting", "private", "", "" } },
            { "한 방", new List<string>() { "FAAC58", "피격당하는 즉시 죽습니다.", "OnePunch", "private", "", "" } },
            { "무법자", new List<string>() { "9F81F7", "모두가 총기 하나를 가지고 시작합니다.", "Outlaw", "public", "", "" } },
            { "해적 룰렛", new List<string>() { "FFBF00", "폭탄을 잘 추려내야 합니다.", "PirateRoulette", "public", "", "" } },
            { "랜덤박스", new List<string>() { "BFFF00", "60초마다 랜덤한 아이템을 얻을 수 있습니다!", "RandomItem", "public", "", "" } },
            { "빨간 불, 초록 불", new List<string>() { "F7D358", "빨간 불에는 움직이지 마세요.", "RedLightGreenLight", "public", "", "" } },
            { "반전", new List<string>() { "A4A4A4", "이동 키(WASD)가 반대로 됩니다.", "Reversal", "private", "world_of_warship_is_ddong_game", "" } },
            { "로켓 런처", new List<string>() { "FA8258", "무슨 이유로든 피격당하면 승천합니다.", "RocketLauncher", "public", "", "" } },
            { "SCP 러쉬", new List<string>() { "FE2E2E", "모든 SCP가 한 개체로 통일됩니다.", "SCPRUSH", "public", "", "" } },
            { "시베리아", new List<string>() { "FAFAFA", "최대한 다른 자들과 붙어 온기를 나눠 가지세요!", "Siberia", "private", "몬키키", "" } },
            { "소울메이트", new List<string>() { "FF00FF", "단짝 친구와 모든 것을 함께하세요!", "SoulMate", "public", "", "" } },
            { "특수능력", new List<string>() { "2EFEF7", "모두가 특수 능력을 지니고 시작합니다.", "SpecialAbility", "private", "", "" } },
            { "스피릿", new List<string>() { "CED8F6", "죽으면 영혼 상태에 돌입합니다!", "Spirit", "public", "", 
"""
죽으면 영혼 상태로 부활합니다. 이 상태에서 사망하면 성불됩니다.
또한, 자살로 사망한 경우 곧바로 성불됩니다.

스피릿은 같은 스피릿을 사살할 수 있으니, 유의하십시오.
""" } },
            { "스플리프", new List<string>() { "BEF781", "떨어지지 않으려면 계속 움직이세요!", "Spleef", "public", "", "" } },
            { "기본템", new List<string>() { "FA5858", "5~8개의 아이템이 사전에 지급됩니다.\n기존에 가지고 있던 아이템은 제거됩니다.", "StandardItems", "private", "", "" } },
            { "슈퍼 스타", new List<string>() { "FE2EF7", "모두의 마이크가 공유됩니다.", "SuperStar", "public", "", "" } },
            { "무덤", new List<string>() { "000000", "살아남으려면 뭐라도 해야 합니다.", "Tomb", "public", "", "" } },
            { "트릭 오어 트릿", new List<string>() { "5F04B4", "사탕 4개를 가지고 시작합니다.\n다른 이를 사살하면 사탕 1개를 더 받습니다.", "TrickorTreat", "public", "", "" } },
            { "무제한", new List<string>() { "3F13AB", "무제한을 악용하지 않는 것을 추천합니다.", "Unlimited", "public", "", "" } },
            { "여긴 어디?", new List<string>() { "B40486", "랜덤한 곳에서 스폰됩니다.", "WhereamI", "public", "", "" } },
            { "나는 누구?", new List<string>() { "886A08", "1분마다 진영이 변경됩니다.", "WhoamI", "public", "", "" } },
        };
    }
}
