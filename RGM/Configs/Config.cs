using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;
using RGM.Modes;

namespace RGM
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public string WebhookURL { get; set; } = "https://discord.com/api/webhooks/1281967055272153099/b4-Qw3t5V4Tliq6axCheK0Ekv7_XdZ25MhbVMbepF_7WPz8RwsXw_c-3S58Cx3c8hj24";
        public string BotAPIServer { get; set; } = "http://127.0.0.1:50000/";
        public string StartModeDescription { get; set; } = "<size=30><b><color=#{ModeColor}>{CurrentMode}</color></b></size>\n<size=25>{ModeDescription}</size>";
        public string LateJoinModeDescription { get; set; } = "<size=20>현재 진행중인 모드</size>\n<size=25><b><color=#{ModeColor}>{CurrentMode}</color></b></size>";
        public string WelcomeMessage { get; set; } = "<size=25><b>랜덤게임모드</b>에 오신 것을 환영합니다!</size>";
        public string LobbyMessage { get; set; } = "\n<align=left>\n[1] {First} | {FirstVote}\n[2] {Second} | {SecondVote}\n[3] {Third} | {ThirdVote}\n</align>\n\n<align=left><color=#{ModeColor}><b>{ModeName}</b></color>\n{ModeDescription}</align>{Lines}\n\n\n\n\n";
    }
}
