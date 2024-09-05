using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;

namespace Plugin
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("모드 이름 : 색상, 설명, 파일 이름, 공개 여부(public/private), 아이디어 제공자")]
        public static Dictionary<string, List<string>> Modes { get; set; } = new Dictionary<string, List<string>>() 
        {
            { "한 방", new List<string>() { "FAAC58", "피격당하는 즉시 죽습니다.", "OnePunch", "public", "" } },
            { "츄파츕스", new List<string>() { "A9E2F3", "모두가 제일버드를 가지고 시작합니다.", "Jailbird", "public", "" } },
            { "SCP 러쉬", new List<string>() { "FE2E2E", "모든 SCP가 한 개체로 통일됩니다.", "SCPRUSH", "public", "" } }
        };

        public string StartModeDescription { get; set; } = "<size=30>[<b><color=#{ModeColor}>{CurrentMode}</color></b>]</size>\n<size=25>{ModeDescription}</size>";
        public string LateJoinModeDescription { get; set; } = "<size=20>현재 진행중인 모드</size>\n<size=25><b>[<color=#{ModeColor}>{CurrentMode}</color>]</b></size>";
        public string WelcomeMessage { get; set; } = "<size=25><b>랜덤게임모드</b>에 오신 것을 환영합니다!</size>";
        public string LobbyMessage { get; set; } = "\n\n\n\n\n\n\n<size=200><b>?</b></size>\n<size=20>\"이번 라운드는 어떤 모드가 걸릴까요?\"</size>\n";
    }
}
