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
            { "워크스테이션 업그레이드", new List<string>() { "00FFFF", "워크스테이션에서 업그레이드하세요!", "ABattle", "public", "" } },
            { "블랙아웃", new List<string>() { "2A0A0A", "시설 곳곳이 정전됩니다.", "Blackout", "public", "" } },
            { "축복", new List<string>() { "F6D8CE", "관전자의 수에 비례해 능력치가 상승합니다.", "Blessing", "public", "" } },
            { "폭탄 파티", new List<string>() { "FAAC58", "버티면 버틸수록 난이도가 올라갑니다.", "BombParty", "public", "" } },
            { "수집가", new List<string>() { "FFBF00", "SCP 아이템 3개를 가지고 시작합니다.", "Collector", "public", "" } },
            { "사회적 거리두기", new List<string>() { "38610B", "최대한 다른 사람과 멀어지세요! 감염 예방이 최우선입니다!", "Distancing", "public", "" } },
            { "개인전", new List<string>() { "FA58F4", "최후의 1인이 되세요!", "FreeForAll", "public", "" } },
            { "어제의 동지는 오늘의 적", new List<string>() { "F78181", "팀킬이 가능합니다, 이젠 아무도 믿지 마세요.\nSCP도 마찬가지입니다!", "FriendlyFire", "public", "" } },
            { "도박", new List<string>() { "8A4B08", "아이템을 떨구면 새로운 아이템을 획득합니다.\n단, 2% 확률로 손이 잘립니다.", "Gamble", "public", "" } },
            { "기브 어웨이", new List<string>() { "E6E6E6", "60초마다 랜덤한 아이템이 지급됩니다.", "Giveaway", "private", "" } },
            { "HIDE", new List<string>() { "0489B1", "숨 죽이는 그를 사살하십시오.", "HIDE", "public", "" } },
            { "GG 클럽", new List<string>() { "C8FE2E", "빠르게 황금색 플랫폼을 사수하세요!", "GGClub", "private", "" } },
            { "폭탄 돌리기", new List<string>() { "FA58D0", "폭탄이 터지기 전에 다른 유저에게 넘기세요!", "HotPotato", "private", "" } },
            { "무한 탄약", new List<string>() { "6E6E6E", "무제한 탄약과 함께 승전보를 울리세요!", "InfiniteAmmo", "private", "" } },
            { "츄파츕스", new List<string>() { "A9E2F3", "모두가 제일버드를 가지고 시작합니다.", "Jailbird", "public", "" } },
            { "존 윅", new List<string>() { "2EFEF7", "권총류 무기의 데미지가 400% 상승합니다.", "JohnWick", "public", "" } },
            { "저거너트", new List<string>() { "088A08", "모두 힘을 합쳐 외부의 적에 대항하세요.\n10분에 자동핵이 작동됩니다.", "Juggernaut", "public", "" } },
            { "점프맵 라운지", new List<string>() { "A9D0F5", "5분 안에 최대한 멀리 가세요!", "JumpMap", "public", "" } },
            { "한국인이 좋아하는 속도", new List<string>() { "5882FA", "이런 거 좋아하시죠?", "KoreanSpeed", "public", "" } },
            { "리더", new List<string>() { "64FE2E", "각 진영마다 리더가 정해집니다.\n리더를 도와 진영을 승리로 이끄세요!", "Leader", "private", "" } },
            { "미니게임", new List<string>() { "6E6E6E", "미니 게임 중 하나가 랜덤으로 선택됩니다. 총 3개의 라운드로 진행됩니다.", "MiniGames", "public", "" } },
            { "중첩", new List<string>() { "04B45F", "HP 제한이 제거됩니다.", "Nesting", "private", "" } },
            { "한 방", new List<string>() { "FAAC58", "피격당하는 즉시 죽습니다.", "OnePunch", "private", "" } },
            { "무법자", new List<string>() { "9F81F7", "모두가 총기 하나를 가지고 시작합니다.", "Outlaw", "public", "" } },
            { "해적 룰렛", new List<string>() { "FFBF00", "폭탄을 잘 추려내야 합니다.", "PirateRoulette", "public", "" } },
            { "랜덤박스", new List<string>() { "BFFF00", "60초마다 랜덤한 아이템을 얻을 수 있습니다!", "RandomItem", "public", "" } },
            { "로켓 런처", new List<string>() { "FA8258", "무슨 이유로든 피격당하면 승천합니다.", "RocketLauncher", "public", "" } },
            { "SCP 러쉬", new List<string>() { "FE2E2E", "모든 SCP가 한 개체로 통일됩니다.", "SCPRUSH", "public", "" } },
            { "시베리아", new List<string>() { "FAFAFA", "최대한 다른 자들과 붙어 온기를 나눠 가지세요!", "Siberia", "public", "몬키키" } },
            { "소울메이트", new List<string>() { "FF00FF", "단짝이 죽으면 자신도 죽습니다.", "SoulMate", "public", "" } },
            { "특수능력", new List<string>() { "2EFEF7", "모두가 특수 능력을 지니고 시작합니다.", "SpecialAbility", "private", "" } },
            { "스피릿", new List<string>() { "CED8F6", "죽으면 영혼 상태에 돌입합니다!", "Spirit", "public", "" } },
            { "기본템", new List<string>() { "FA5858", "5~8개의 아이템이 사전에 지급됩니다.\n기존에 가지고 있던 아이템은 제거됩니다.", "StandardItems", "private", "" } },
            { "슈퍼 스타", new List<string>() { "FE2EF7", "모두의 마이크가 공유됩니다.", "SuperStar", "public", "" } },
            { "무덤", new List<string>() { "000000", "살아남으려면 뭐라도 해야 합니다.", "Tomb", "public", "" } },
            { "트릭 오어 트릿", new List<string>() { "5F04B4", "사탕 4개를 가지고 시작합니다.\n다른 이를 사살하면 사탕 1개를 더 받습니다.", "TrickorTreat", "public", "" } },
            { "무제한", new List<string>() { "3F13AB", "무제한을 악용하지 않는 것을 추천합니다.", "Unlimited", "public", "" } },
            { "여긴 어디?", new List<string>() { "B40486", "랜덤한 곳에서 스폰됩니다.", "WhereamI", "public", "" } },
            { "나는 누구?", new List<string>() { "886A08", "1분마다 진영이 변경됩니다.", "WhoamI", "public", "" } },
            { "더블업", new List<string>() { "5882FA", "모드 2개가 동시에 적용됩니다.", "DoubleUp", "public", "" } },
        };
    }
}
