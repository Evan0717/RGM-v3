using RGM.API.Features;
using RGM.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using PlayerRoles;
using Exiled.API.Extensions;

namespace RGM.Variables
{
    public static class ServerManagers
    {
        public static ModeType CurrentMode = ModeType.None;
        public static ModeType CurrentSubMode = ModeType.None;
        public static AudioPlayer GlobalPlayer;
        public static string SelectMode = null;
        public static string Tip = Tools.GetRandomValue(Tips.LobbyTips);
        public static string Logo = UnityEngine.Random.Range(1, 3) == 1 ? "❓" : "❔";
        public static int StartupRandom = UnityEngine.Random.Range(1, 31);
        public static bool FreezeGameStart = false;
        public static bool AutoNuke = false;
        public static bool IsScp3114Enabled = false;
        public static bool IsBugVoteProcessing = false;
        public static bool IsUsersFileLoaded = false;
        public static bool IsDropScpItemAllowed = true;
        public static bool IsWinnerSelected = false;
        public static bool IsWarningAlone = false;
        public static bool IsClearCitizen = false;

        public static Dictionary<ModeType, ModeData> ModeList = new Dictionary<ModeType, ModeData>();
        public static Dictionary<ModeType, List<Player>> ModeVote = new Dictionary<ModeType, List<Player>>();
        public static Dictionary<Player, float> OnGround = new Dictionary<Player, float>();
        public static Dictionary<Player, Room> CurrentRoom = new Dictionary<Player, Room>();
        public static Dictionary<string, PlayerInfo> PlayersInfo = new Dictionary<string, PlayerInfo>();
        public static Dictionary<string, PlayerReport> PlayersReport = new Dictionary<string, PlayerReport>();
        public static Dictionary<Door, int> InteractedDoors = new Dictionary<Door, int>();
        public static Dictionary<string, string> KillEffects = new Dictionary<string, string>()
        {
            {"영혼 가출", "죽은 상대에게서 혼을 추출해냅니다!"},
            {"솔라 테라", "죽음에 햇빛 한 점 들기를.."},
            {"Kerfus", "귀여운 로봇으로 도장을 찍어보세요."},
            {"은제 말뚝", "비수를 꽂는 것처럼 소름끼칩니다!"},
            {"KO 사인", "넉 다운! 상대를 쓰러트리세요!"},
            {"크리스마스 트리", "축제를 돋보이게 하는 아담한 트리입니다."},
            {"크리스마스 볼", "축제의 연출을 돕는 스노우볼입니다."}
        };
        public static Dictionary<string, string> Customizations = new Dictionary<string, string>()
        {
            {"커스텀 닉네임", "표시되는 플레이어 이름을 수정합니다."},
            {"커스텀 인포", "플레이어 인포를 추가합니다."}
        };
        public static Dictionary<string, string> Paints = new Dictionary<string, string>()
        {
            {"블랙골드", "검은색과 금색의 달콤한 콜라보!"},
            {"핫핑크", "두근두근거리는 핑크들의 콜라보!"},
            {"레인보우", "R.A.I.N.B.O.W."},
            {"분홍색", "이 느낌은 뭔가 편안함을 줍니다."},
            {"빨간색", "강렬한 느낌!"},
            {"흰색", "기본적인 색상입니다. 도화지 같다고나 할까요."},
            {"갈색", "달콤하고도 안정적인 무언가가 생각나네요."},
            {"은색", "은의 힘을 받아 혈을 물리칩니다."},
            {"밝은 녹색", "녹색이 밝으면 보기 좋죠."},
            {"진홍색", "찐한 분홍색! 색다른 느낌이죠?"},
            {"청록색", "진한 초록색! 어두운 진실이 감춰진 것 같습니다."},
            {"옥색", "청량한 바다가 연상되네요."},
            {"진한 분홍색", "진한 분홍색? 으음.."},
            {"토마토색", "나는야 멋쟁이 토 마 토"},
            {"노란색", "GoldenPig1205가 좋아하는 색상입니다."},
            {"짙은 홍색", "시산혈해"},
            {"푸른 녹색", "파릇 파릇하니, 기분이 좋군요."},
            {"주황색", "결심이 확실한 색상이에요."},
            {"라임색", "라임 색상은 라임을 잘 맞춰..ㅋㅋ"},
            {"초록색", "자연이 연상되는 색이군요."},
            {"에메랄드색", "에메랄드보다 더 희귀한 광석은 무엇일까요? (웃음)"},
            {"카민색", "이건 뭔 색이지"},
            {"니켈색", "어떤 색이랑 많이 비슷하군요."},
            {"박하색", "민트 초코 좋아하신다구요?"},
            {"군대 녹색", "이름이 왜 군대(army) 녹색일까요?"},
            {"호박색", "할로윈 좋아하세요?"}
        };
        public static Dictionary<string, string> Badges = new Dictionary<string, string>()
        {
            {"RGM Owner", "랜덤게임모드(RGM) 공식 운영자 칭호"},
            {"RGM Administrator", "랜덤게임모드(RGM) 공식 관리자 칭호"},
            {"RGM Developer", "랜덤게임모드(RGM) 공식 개발자 칭호"},
            {"호기심 많은 자", "상점에 칭호가 있어서 사봤어요!"},
            {"Merry Christmas", "즐거운 크리스마스 보내세요."},
            {"1st Anniversary", "랜덤게임모드 1주년을 기념하는 이벤트 우승자"},
            {"Adieu, Polaris", "초대 개발자의 마지막 이벤트 우승자"},
            {"Adieu! 2023", "2023년의 마지막을 기념하며"},
            {"2023 RGM Summer", "2023년도 여름에 진행되었던 이벤트 우승자"},
            {"꾸요미 D계급", "D계급은 기본적으로 귀엽게 생겼다고 할 수 있습니다."},
            {"야근중인 과학자", "오늘도 대업을 위해 일을 멈추지 않는 과학자입니다."},
            {"복면이 그리운 시설 경비", "이런! 한기가 느껴지는 재단에서는 복면이 참 좋았는데.."},
            {"무능한 구미호", "너무 무능하다고 하지 마세요. 그래도 총은 있잖.. 어?"},
            {"난동꾼 반란", "\"시설에 난입하여 그들만의 이익을 취합니다.\""},
            {"동네북 뱀의 손", "너는 누구 팀이야? SCP? 인간?"},
            {"Adios! 2024", "2024년의 마지막을 기념하며"}
        };
        public static Dictionary<string, string> BadgeIcons = new Dictionary<string, string>()
        {
            {"RGM Owner", "👑"},
            {"RGM Administrator", "⚖️"},
            {"RGM Developer", "🔧"},
            {"호기심 많은 자", "❓"},
            {"1st Anniversary", "⭐"},
            {"Adieu, Polaris", "⭐"},
            {"Adieu! 2023", "⭐"},
            {"2023 RGM Summer", "⭐"},
            {"Adios! 2024", "✿"}
        };

        public static List<Transform> First;
        public static List<Transform> Second;
        public static List<Transform> Third;
        public static List<Transform> Numbers;
        public static List<Transform> RandomColors;
        public static List<Transform> RandomLights;
        public static List<Transform> Balls;
        public static List<ModeType> EnabledModeList = new List<ModeType>();
        public static List<ModeType> SubModeVote = new List<ModeType>();
        public static List<string> Requests = new List<string>();
        public static List<Player> JumpScareCooldown = new List<Player>();
        public static List<Player> GodModePlayers = new List<Player>();
        public static List<Player> ChatCooldown = new List<Player>();
        public static List<Player> EmotionCooldown = new List<Player>();
        public static List<Player> BugVotePlayers = new List<Player>();
        public static List<Player> BugVoteUsers = new List<Player>();
        public static List<Player> IntercomPlayers = new List<Player>();
        public static List<Player> ShopCooldown = new List<Player>();
        public static List<Product> Products = new List<Product>()
        {
            new Product()
            {
                Name = "인형 소환",
                Description = "랜덤한 역할군의 인형을 소환합니다. 로비에서만 사용할 수 있습니다.",
                Price = 3,
                Check = (player) => { return !Round.IsStarted; },
                Script = (player) =>
                {
                    Ragdoll.CreateAndSpawn(Tools.EnumToList<RoleTypeId>().Where(x => x.TryGetRoleBase(out PlayerRoleBase roleBase)).GetRandomValue(), "인형", "이 깜찍한 인형 좀 보세요.", player.Position);
                }
            }
        };
    }
}
