using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;

namespace RGM
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = false;
        public bool Debug { get; set; } = false;

        public string WebhookURL { get; set; } = "https://discord.com/api/webhooks/1281967055272153099/b4-Qw3t5V4Tliq6axCheK0Ekv7_XdZ25MhbVMbepF_7WPz8RwsXw_c-3S58Cx3c8hj24";
        public string BotAPIServer { get; set; } = "http://127.0.0.1:50000/";
        public string StartModeDescription { get; set; } = "<size=30>[<b><color=#{ModeColor}>{CurrentMode}</color></b>]</size>\n<size=25>{ModeDescription}</size>";
        public string LateJoinModeDescription { get; set; } = "<size=20>현재 진행중인 모드</size>\n<size=25><b>[<color=#{ModeColor}>{CurrentMode}</color>]</b></size>";
        public string WelcomeMessage { get; set; } = "<size=25><b>랜덤게임모드</b>에 오신 것을 환영합니다!</size>";
        public string LobbyMessage { get; set; } = "\n<align=left>\n[1] {First} | {FirstVote}\n[2] {Second} | {SecondVote}\n[3] {Third} | {ThirdVote}\n</align>\n\n\n<size=200><b>?</b></size>\n<size=20>\"{ModeDescription}\"</size>\n";

        [Description("모드 이름 : 색상, 설명, 파일 이름, 공개 여부(public/private), 아이디어 제공자")]
        public Dictionary<string, List<string>> Modes { get; set; } = new Dictionary<string, List<string>>()
        {
            { "블랙아웃", new List<string>() { "2A0A0A", "시설 곳곳이 정전됩니다.", "Blackout", "public", "" } },
            { "축복", new List<string>() { "F6D8CE", "관전자의 수에 비례해 능력치가 상승합니다.", "Blessing", "public", "" } },
            { "수집가", new List<string>() { "FFBF00", "SCP 아이템 3개를 가지고 시작합니다.", "Collector", "public", "" } },
            { "어제의 동지는 오늘의 적", new List<string>() { "F78181", "팀킬이 가능합니다, 이젠 아무도 믿지 마세요.\nSCP도 마찬가지입니다!", "FriendlyFire", "public", "" } },
            { "도박", new List<string>() { "8A4B08", "아이템을 떨구면 새로운 아이템을 획득합니다.\n단, 2% 확률로 손이 잘립니다.", "Gamble", "public", "" } },
            { "기브 어웨이", new List<string>() { "E6E6E6", "60초마다 랜덤한 아이템이 지급됩니다.", "Giveaway", "public", "" } },
            { "폭탄 돌리기", new List<string>() { "FA58D0", "폭탄이 터지기 전에 다른 유저에게 넘기세요!", "HotPotato", "private", "" } },
            { "무한 탄약", new List<string>() { "6E6E6E", "무제한 탄약과 함께 승전보를 울리세요!", "InfiniteAmmo", "public", "" } },
            { "츄파츕스", new List<string>() { "A9E2F3", "모두가 제일버드를 가지고 시작합니다.", "Jailbird", "public", "" } },
            { "존 윅", new List<string>() { "2EFEF7", "권총류 무기의 데미지가 400% 상승합니다.", "JohnWick", "public", "" } },
            { "한국인이 좋아하는 속도", new List<string>() { "5882FA", "이런 거 좋아하죠?", "KoreanSpeed", "public", "" } },
            { "리더", new List<string>() { "64FE2E", "각 진영마다 리더가 정해집니다.\n리더를 도와 진영을 승리로 이끄세요!", "Leader", "public", "" } },
            { "중첩", new List<string>() { "04B45F", "HP 제한이 제거됩니다.", "Nesting", "public", "" } },
            { "한 방", new List<string>() { "FAAC58", "피격당하는 즉시 죽습니다.", "OnePunch", "public", "" } },
            { "무법자", new List<string>() { "9F81F7", "모두가 총기 하나를 가지고 시작합니다.", "Outlaw", "public", "" } },
            { "해적 룰렛", new List<string>() { "FFBF00", "폭탄을 잘 추려내야 합니다.", "PirateRoulette", "public", "" } },
            { "SCP 러쉬", new List<string>() { "FE2E2E", "모든 SCP가 한 개체로 통일됩니다.", "SCPRUSH", "public", "" } },
            { "기본템", new List<string>() { "FA5858", "5~8개의 아이템이 사전에 지급됩니다.\n기존에 가지고 있던 아이템은 제거됩니다.", "StandardItems", "public", "" } },
            { "트릭 오어 트릿", new List<string>() { "5F04B4", "사탕 4개를 가지고 시작합니다.\n다른 이를 사살하면 사탕 1개를 더 받습니다.", "TrickorTreat", "public", "" } },
            { "여긴 어디?", new List<string>() { "B40486", "랜덤한 곳에서 스폰됩니다.", "WhereamI", "public", "" } },
            { "나는 누구?", new List<string>() { "886A08", "1분마다 진영이 변경됩니다.", "WhoamI", "public", "" } },
            { "더블업", new List<string>() { "5882FA", "모드 2개가 동시에 적용됩니다.", "DoubleUp", "public", "" } },
            { "특수능력", new List<string>() { "2EFEF7", "모두가 특수 능력을 지니고 시작합니다.", "SpecialAbility", "public", "" } }
        };
    }
}
