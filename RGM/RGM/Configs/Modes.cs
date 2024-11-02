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
<color=#F5DA81>인간 진영</color>일 경우, 워크스테이션에서 점프하면 능력을 1개 얻습니다.
<color=red>SCP-079</color>일 경우, 레벨이 올라갈 때마다 능력을 1개 얻습니다.

각 능력 등급들의 확률을 확인하려면 아래를 참고하십시오.
• <color=#A4A4A4>일반</color> - 70%
• <color=#2ECCFA>희귀</color> - 24.5%
• <color=#FF00FF>영웅</color> - 5%
• <color=#ffd700>전설</color> - 0.4%
• <color=#DF0101>신화</color> - 0.1%
• <color=#F7819F>전용</color> - 5% (능력 선택 옵션 독립)
• <color=##DEEFED>시너지</color> - ???

20% 확률로 피버 모드가 활성화됩니다.
<b><i><color=#FF00EA>피</color><color=#EF00EB>버</color> <color=#CF00ED>모</color><color=#BF00EF>드</color></i></b> 활성화 시, 재단에 등장하는 워크스테이션의 수가 증가합니다.
""" } },
            { "추가 지원", new List<string>() { "F5D0A9", "지원의 형태가 추가되고, 추가 아이템이 지급됩니다.", "", "onlysub", "AdditionalWave",
"""
[지원 형태]
<color=#F781F3>뱀의 손</color>
<color=red>SCP-049-2</color>

[지원 아이템 목록]
최대 3개까지 모든 아이템 중 랜덤으로 선택됨
""" } },
            { "블랙아웃", new List<string>() { "2A0A0A", "시설 곳곳이 정전됩니다.", "Blackout", "onlysub", "",
"""
각 방마다 <color=#FF00FF>기믹</color>이 적용될 수 있습니다.

50% - 방이 정전됩니다.
50% - 방의 색상이 변경됩니다.
(소숫점 버림)
""" } },
            { "축복", new List<string>() { "F6D8CE", "관전자의 수에 비례해 능력치가 상승합니다.", "Blessing", "public", "",
"""
플레이어는 자신을 바라보는 <b>관전자</b>의 수를 확인할 수 있습니다.
<b>관전자</b>의 수에 비례해 다음 능력치가 상승합니다.
• 이동 속도
• 피해량 증가
• 데미지 감소
• 초당 체력 회복
• (SCP-079) 전력 회복

<b>관전자</b>의 수가 5명씩 넘어갈 때마다 특수한 효과가 적용됩니다.
6명 이상 - 스테미나 무제한
11명 이상 - 바이패스 활성화
16명 이상 - 유령화 효과
21명 이상 - <i><color=#A400F0>투</color><color=#B600EE>명</color> <color=#DA00EC>효</color><color=#EC00EB>과</color></i>
26명 이상 - <b><color=#57F104>아</color><color=#5DEE03>이</color><color=#63EB03>템</color><color=#6AE803>이</color> <color=#76E202>지</color><color=#7DDF02>급</color><color=#83DD01>될</color> <color=#90D701>수</color> <color=#9DD100>있</color><color=#A3CE00>음</color></b>
31명 이상 - <b><i><color=#2718F7>노</color><color=#222DEF>클</color><color=#1E42E7>립</color> <color=#156CD8>사</color><color=#1181D1>용</color> <color=#08ABC2>가</color><color=#04C0BA>능</color></i></b>
""" } },
            { "폭탄 파티", new List<string>() { "FAAC58", "버티면 버틸수록 난이도가 올라갑니다.", "BombParty", "public", "",
"""
투사체 또는 SCP 아이템이 랜덤한 곳에 떨어집니다.

3초마다 <color=#F7BE81>이벤트</color>가 일어납니다.

라운드 경과
0초 후ㅣ<b>고폭 수류탄</b>이 떨어집니다.
30초 후ㅣ<b>섬광탄</b>이 1/3 확률로 떨어집니다.
60초 후ㅣ<b>고폭 수류탄</b>이 1/3 확률로 떨어집니다.
90초 후ㅣ<b>SCP-244</b>가 1/3 확률로 떨어집니다.
120초 후ㅣ<b>SCP-018</b>이 1/3 확률로 떨어집니다.

3분 이상 버틴다면 스스로를 칭찬해주세요.
""" } },
            { "고문", new List<string>() { "D7DF01", "고문을 최대한 오래 버티는 플레이어가 승리합니다.", "Cell", "public", "",
"""
좁은 방에서 <color=#FE2E2E>SCP-018</color>이 던져집니다!

최대한 잘 피해 보세요!
""" } },
            { "수집가", new List<string>() { "FFBF00", "SCP 아이템 3개를 가지고 시작합니다.", "Collector", "public", "",
"""
이름에 <color=#FE2E2E>SCP</color>가 들어가는 모든 아이템 중에서 랜덤으로 3개를 지급받고 시작합니다.

<color=#FE2E2E>SCP</color>의 경우에는 하나의 <b><color=#FE2E2E>SCP</color> 아이템</b>만 지급받습니다.
""" } },
            { "데드 라인", new List<string>() { "FA8258", "빨간색을 밟지 마세요!", "DeadLine", "public", "",
"""
<color=red>빨간색</color>을 밟으면 죽습니다.

<i>발 아래를 조심하세요!</i>
""" } },
            { "데스런", new List<string>() { "FF4000", "과학자는 죄수들의 접근을 막아야 합니다. 널리 퍼져 있는 함정들을 조심하십시오!", "DeathRun", "private", "",
"""
<color=yellow>과학자</color>의 경우, <color=red>빨간 버튼</color>을 눌러 함정을 발동시킬 수 있습니다.
단, 함정은 1번만 작동시킬 수 있으므로, 한번에 최대한 많은 <color=orange>D계급</color>들을 죽이세요.
모든 <color=orange>D계급</color>을 죽이거나, 180초를 버티면 <color=yellow>과학자</color>의 승리입니다.

<color=orange>D계급</color>의 경우, 목적지까지 빠르게 도달해야 합니다.
목적지에 도달한 경우 <color=yellow>과학자</color>를 사살하여 승리할 수 있게 됩니다. 총을 얻게 되는 거죠!
""" } },
            { "사회적 거리두기", new List<string>() { "38610B", "최대한 다른 사람과 멀어지세요! 감염 예방이 최우선입니다!", "Distancing", "onlysub", "", 
"""
<i>다른 사람과 가까이 붙지 마세요.</i>

7.5m 초과의 거리를 유지하지 못한다면 체력의 일정 비율만큼 데미지를 입습니다.
""" } },
            { "더블업", new List<string>() { "5882FA", "모드 2개가 동시에 적용됩니다. <size=20>(모드셋은 최대 하나까지만 적용됩니다.)</size>", "DoubleUp", "public", "", 
"""
<b>모드셋은 모드에이드와 상반되는 모드 종류입니다.</b>

모드셋 - 기존 SCP:SL의 게임 방식의 궤가 크게 달라짐. (ex. 나는 누구?, 스플리프)
모드에이드 - 기존 SCP:SL의 게임 방식에 새로운 요소를 더함. (ex. 도박, 존 윅)
서브모드 - 서브로만 등장하는 모드입니다. (ex. 한국인이 좋아하는 속도, 시베리아)

<i>* 서브 모드는 하나만 적용될 경우 게임의 재미가 떨어질 수도 있는 모드 집합입니다.</i>

모드 조합은 다음과 같이 등장할 수 있습니다.
- [모드셋, 모드에이드]
- [모드에이드, 모드에이드]
- [모드셋, 서브모드]
- [모드에이드, 서브모드]
- [서브모드, 서브모드]
""" } },
            { "개인전", new List<string>() { "FA58F4", "최후의 1인이 되세요!", "FreeForAll", "public", "", 
"""
랜덤한 문으로 순간이동한 후 랜덤하게 지급되는 아이템으로 싸움을 시작합니다.

<i>모든 문은 잠겨 있습니다.</i>
""" } },
            { "어제의 동지는 오늘의 적", new List<string>() { "F78181", "팀킬이 가능합니다, 이젠 아무도 믿지 마세요.", "FriendlyFire", "public", "",
"""
<color=#81F781>인간</color>이던 <color=red>SCP</color>던 <b><color=#FFBF00>팀킬</color>이 허용</b>됩니다.
팀킬이 허용되므로, 티밍이 자유로워집니다. (이 모드는 팀킬과 티밍으로 제재되지 않습니다.)

인간 진영의 경우 [ALT]키를 눌러 다른 플레이어에게 주먹을 날릴 수 있습니다. (데미지: 12.05)
SCP 진영의 경우 기본 공격으로 다른 플레이어를 공격할 수 있게 되지만 몇몇 SCP는 특수한 규칙을 따릅니다.
- <color=red>SCP-173</color> 순간이동: SCP에게 순간이동할 때, 해당 SCP를 쳐다보고 있어야 목을 꺾을 수 있습니다.
- <color=red>SCP-173</color> 공격: 쳐다보고 있지 않을 때, [ALT]키를 눌러 죽일 수 있습니다.
- <color=red>SCP-939</color> 런지: SCP에게 런지를 시도할 때, 해당 SCP를 쳐다보고 있어야 공격이 가능합니다.
- <color=red>SCP-106</color> 공격: [ALT]키를 눌러 공격을 시도할 수 있습니다.
- <color=red>SCP-3114</color> 목 조르기: 불가

<b><i><color=#AEFF00>개</color><color=#B7E200>발</color><color=#C0C600>자</color> <color=#D28D00>추</color><color=#DB7100>천</color> <color=#ED3800>모</color><color=#F61C00>드</color></i></b> 🥵🥵
<size=15>내면 속 악마의 속삭임에 몸을 맡기세요</size>
""" } },
            { "도박", new List<string>() { "8A4B08", "아이템을 떨구면 새로운 아이템을 획득합니다. 단, 2% 확률로 손이 잘립니다.", "Gamble", "public", "", 
"""
생각 없이 도박을 하다 보면 2%는 금방이랍니다.
""" } },
            { "Gun Game", new List<string>() { "088A08", "대인전에 자신 있으신가요? 실력을 증명하기 위해 우승을 차지하세요!", "GunGame", "public", "",
"""
제일버드
입자 분열기
FR-MG-0
Logicer
MTF-E11-SR
AK
Crossvec
A7
FSP-9
COM-45
.44 리볼버
COM-18
COM-15
마이크로 H.I.D
""" } },
            { "HIDE", new List<string>() { "0489B1", "숨 죽이는 그를 사살하십시오.", "HIDE", "public", "", 
"""
<color=red>SCP-3114</color>는 피격당하거나 공격하면 투명이 해제됩니다.
또한, 평상시에도 반투명 상태로 존재합니다. 이 점을 잘 이용해보세요!
""" } },
            { "GG 클럽", new List<string>() { "C8FE2E", "빠르게 황금색 플랫폼을 사수하세요!", "GGClub", "public", "",
"""
<b><color=#F00000>페</color><color=#F54900>이</color><color=#FA9300>즈</color></b>가 총 10개로 이루어져 있습니다!

순발력을 마음껏 뽐내 보세요!
""" } },
            { "숨바꼭질", new List<string>() { "F5A9E1", "꼭꼭 숨으세요! 사냥개가 당신을 찾을 것입니다. 제한 시간동안 버티세요!", "HideAndSeek", "public", "", 
"""
고도의 심리전 싸움입니다.

최후의 승자는 과연 누가 될 것인가..
""" } },
            { "폭탄 돌리기", new List<string>() { "FA58D0", "폭탄이 터지기 전에 다른 유저에게 넘기세요!", "HotPotato", "public", "", 
"""
유저에 비례하여 <b>폭탄 플레이어의 수가 조정</b>됩니다. (10명 당 1마리 + 1마리)

어떤 수단을 사용하더라도 최후까지 살아남으세요!
""" } },
            { "존 윅", new List<string>() { "2EFEF7", "권총류 무기의 데미지가 400% 상승합니다.", "JohnWick", "onlysub", "", 
"""
COM-15
COM-18
COM-45
.44 리볼버
""" } },
            { "저거너트", new List<string>() { "088A08", "모두 힘을 합쳐 외부의 적에 대항하세요.", "Juggernaut", "public", "",
"""
지금은 우리(SCP, 반란, MTF)끼리 싸울 때가 아닙니다.
군대를 홀로 섬멸할 수 있는 전력인 저거너트가 재단을 점령하기 위해 왔습니다.

<i>이제 친구가 될 시간이야.</i>

<i>* 게임 시작 10분 뒤 <color=red>자동핵</color>이 작동됩니다.</i>
""" } },
            { "점프맵 라운지", new List<string>() { "A9D0F5", "5분 안에 최대한 멀리 가세요!", "JumpMap", "public", "", 
"""
스테이지가 총 11라운드로 이루어져 있습니다.

7 스테이지의 경우에는 고수만 해법을 찾을 수 있습니다.
되도록이면 중앙으로 가거나, 넉백되어도 괜찮을 만큼 여유를 두십시오.
""" } },
            { "미니게임", new List<string>() { "A4A4A4", "간단한 게임들을 즐겨보세요! 총 3개의 라운드로 구성되어 있습니다.", "MiniGames", "public", "",
"""
등장하는 미니게임들의 목록입니다.

airstrike
dm
escape
battle
versus
cs
glass
deathrun
line
dodge
fall
football
gungame
knives
puzzle
race
light
spleef
tag
tdm
lava
zombie
zombie2
""" } },
            { "한국인이 좋아하는 속도", new List<string>() { "5882FA", "이런 거 좋아하시죠?", "KoreanSpeed", "onlysub", "",
"""
<b><i><color=#FB00FF>슈</color><color=#D200D5>우</color><color=#A901AB>우</color><color=#800282>우</color><color=#570358>웅</color><color=#2E042E>화</color></i></b>
""" } },
            { "무법자", new List<string>() { "9F81F7", "모두가 총기 하나를 가지고 시작합니다.", "Outlaw", "public", "", 
"""
남에게 지속적으로 데미지를 입힐 수 있는,
투사체가 아닌 아이템 중에서 랜덤으로 지급받습니다.
""" } },
            { "종이 인간", new List<string>() { "F5A9BC", "종이처럼 펄럭펄럭", "PaperMan", "onlysub", "",
"""
xㅣ0.01
yㅣ1
zㅣ1
의 몸을 가졌습니다!
""" } },
            { "해적 룰렛", new List<string>() { "FFBF00", "폭탄을 잘 추려내야 합니다.", "PirateRoulette", "private", "", 
"""
운빨 싸움
""" } },
            { "05 평의회 구출 작전", new List<string>() { "0040FF", "05 평의회를 구출하려는 자들과 사살하려는 자들의 싸움", "Rescue05", "public", "",
"""
<color=#000000><b>05 평의회</b></color>가 탈출에 성공할 경우,
<color=#2E9AFE>MTF</color> 진영은 <b>강화제 제작 방법</b>을 입수하게 됩니다.

<color=#000000><b>05 평의회</b></color>가 사망할 경우,
<color=#088A08>혼돈의 반란</color> 진영만 시설에 지원하게 됩니다.

<i>* 게임 시작 10분 뒤 <color=red>자동핵</color>이 작동됩니다.</i>
""" } },
            { "러시안 룰렛", new List<string>() { "F5ECCE", "", "RussianRoulette", "private", "",
"""
최대 7개의 그룹으로 나뉘어 각 5명씩 예선전을 치릅니다.

각 그룹의 우승자는 최대 7명으로 결승전을 치르게 됩니다.
""" } },
            { "랜덤효과", new List<string>() { "BFFF00", "60초마다 랜덤한 효과를 얻을 수 있습니다!", "RandomEffect", "onlysub", "몬키키", 
"""
1분마다 랜덤한 효과를 지급받을 수 있습니다.
최대 60만큼, 60초 동안 받습니다.
""" } },
            { "랜덤박스", new List<string>() { "BFFF00", "60초마다 랜덤한 아이템을 얻을 수 있습니다!", "RandomItem", "public", "", 
"""
무작위 아이템들이 동일한 확률로 지급됩니다.

이후, 60초마다 무작위 아이템들을 하나 더 받습니다.
""" } },
            { "랜덤크기", new List<string>() { "BFFF00", "스폰 시 랜덤한 크기로 조정됩니다.", "RandomSize", "onlysub", "",
"""
x: 0.1 ~ 1.2
y: 0.3 ~ 1.2
z: 0.1 ~ 1.2
""" } },
            { "빨간 불, 초록 불", new List<string>() { "F7D358", "빨간 불에는 움직이지 마세요.", "RedLightGreenLight", "public", "", 
"""
<i>순발력이 좋아야 살아남습니다.</i>

엘레베이터와 같은 움직이는 물체를 주의하세요!

아, 그리고 설마 죽을까봐 움직이지 않거나 조금씩만 이동하는 쫄보는 없겠죠?
""" } },
            { "로켓 런처", new List<string>() { "FA8258", "무슨 이유로든 피격당하면 일정 확률로 승천합니다.", "RocketLauncher", "public", "", 
"""
공격자가 <b>인간</b>인 경우 - 5%
공격자가 <b>SCP</b>인 경우 - 50%
공격자가 <b>???</b>인 경우 - 2000%!!!!!
""" } },
            { "SCP 러쉬", new List<string>() { "FE2E2E", "모든 SCP가 한 개체로 통일됩니다.", "SCPRUSH", "public", "", 
"""
SCP-3114도 동일한 확률로 러쉬에 참여할 수 있습니다.
""" } },
            { "시베리아", new List<string>() { "FAFAFA", "최대한 다른 자들과 붙어 온기를 나눠 가지세요!", "Siberia", "onlysub", "몬키키", 
"""
SCP-079가 시설 내 냉각 장치와 에어컨을 풀로 틀어버렸습니다.

3m 이상 떨어지지 마세요!
대상이 누구든 절대로 떨어지지 마십시오.
""" } },
            { "소울메이트", new List<string>() { "FF00FF", "단짝 친구와 모든 것을 함께하세요!", "SoulMate", "public", "",
"""
<b><i><color=#FF00DD>소</color><color=#EB01CD>울</color><color=#D702BD>메</color><color=#C404AD>이</color><color=#B0059D>트</color> <color=#89077D>메</color><color=#76096D>이</color><color=#620A5D>킹</color></i></b>을 시도할 때,
체력이 높은 쪽으로 지정됩니다.
""" } },
            { "스피드런", new List<string>() { "58FAAC", "누구보다 빠르게 시설에서 탈출하세요. 아, 그리고 핵을 중지하려 시도하지 마세요!", "SpeedRun", "public", "",
"""
아이템이 <color=#C8FE2E>랜덤</color>하게 지급됩니다.

모든 방법을 총동원하여 1등으로 시설에서 탈출하세요.
그것 뿐입니다.
""" } },
            { "스피릿", new List<string>() { "CED8F6", "죽으면 영혼 상태에 돌입합니다!", "Spirit", "public", "", 
"""
죽으면 영혼 상태로 부활합니다. 이 상태에서 사망하면 성불됩니다.
또한, 자살로 사망한 경우 곧바로 성불됩니다.
""" } },
            { "Spooky!", new List<string>() { "3104B4", "시작 시 영구적인, 랜덤한 할로윈 효과를 받으세요!", "Spooky", "onlysub", "",
"""
댜음 효과 중 하나의 효과가 영구적으로 적용됩니다.

SugarRush
SugarHigh
SugarCrave
Spicy
OrangeCandy
OrangeWitness
Metal
Marshmallow
Ghostly
Prismatic
OrangeWitness
""" } },
            { "스플리프", new List<string>() { "BEF781", "떨어지지 않으려면 계속 움직이세요!", "Spleef", "public", "", 
"""
총 10개의 층으로 이루어져 있는 것 같습니다.

최대한 오래 살아남기 위한 전략을 고안해 보세요!
""" } },
            { "슈퍼 스타", new List<string>() { "FE2EF7", "모두의 마이크가 공유됩니다.", "SuperStar", "public", "", 
"""
말 그대로입니다. 마이크가 공유됩니다.
""" } },
            { "꼬리 잡기", new List<string>() { "A9F5BC", "절대로 타깃이 아닌 유저를 쏘지 마세요! 꼬리가 잡히지 않도록 하십시오.", "TailCatcher", "public", "", 
"""
<color=red>꼬리가 아닌 유저를 공격하면 데미지가 반사됩니다.</color>

눈치 게임과 비슷하군요!
""" } },
            { "무덤", new List<string>() { "000000", "살아남으려면 뭐라도 해야 합니다.", "Tomb", "public", "", 
"""
널리 펼쳐진 평지에는 <b>수많은 아이템</b>이 널려 있습니다.

<i>배틀그라운드와 흡사하죠.</b>
""" } },
            { "트릭 오어 트릿", new List<string>() { "5F04B4", "사탕 4개를 가지고 시작합니다. 다른 이를 사살하면 사탕 1개를 더 받습니다.", "TrickorTreat", "public", "",
"""
<b><i><color=#E65000>무</color><color=#E5560F>작</color><color=#E55D1F>위</color> <color=#E46A3E>사</color><color=#E4714D>탕</color> <color=#E37E6C>4</color><color=#E3857C>개</color></i></b>를 획득합니다.
<color=#FA58F4>핑크 캔디</color>도 다른 사탕과 동일한 확률로 등장합니다.
""" } },
            { "무제한", new List<string>() { "3F13AB", "무제한을 악용하지 않는 것을 추천합니다.", "Unlimited", "onlysub", "", 
"""
말 그대로 <b>제한</b>이 없습니다.
단, 일부 기능은 쾌적한 플레이를 위해 제한이 있습니다.
""" } },
            { "여긴 어디?", new List<string>() { "B40486", "랜덤한 곳에서 스폰됩니다.", "WhereamI", "public", "", 
"""
여기 어디? []
""" } },
            { "나는 누구?", new List<string>() { "886A08", "1분마다 진영이 변경됩니다.", "WhoamI", "public", "몬키키", 
"""
[] 나는 누구?
""" } },
        };
    }
}
