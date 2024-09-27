using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM
{
    public class Notions
    {
        public static string StartModeDescription { get; set; } = "<size=30><b><color=#{ModeColor}>{CurrentMode}</color></b></size>\n<size=25>{ModeDescription}</size>\n<size=15>콘솔(~ 또는 `)을 열고 자세한 설명을 확인해보세요.</size>";
        public static string LateJoinModeDescription { get; set; } = "<size=20>현재 진행중인 모드</size>\n<size=25><b><color=#{ModeColor}>{CurrentMode}</color></b></size>\n<size=20>{ModeDescription}</size>\n<size=15>콘솔(~ 또는 `)을 열고 자세한 설명을 확인해보세요.</size>";
        public static string WelcomeMessage { get; set; } = "<size=25><b>랜덤게임모드</b>에 오신 것을 환영합니다!</size>";
        public static string LobbyMessage { get; set; } = "\n<align=left>\n{FirstMark} [1] {First} | {FirstVote}\n{SecondMark} [2] {Second} | {SecondVote}\n{ThirdMark} [3] {Third} | {ThirdVote}\n</align>\n\n<align=left><color=#{ModeColor}><b>{ModeName}</b></color>\n{ModeDescription}</align>{Lines}\n\n\n\n<size=25>tip. <i>{Tip}</i></size>";
    }
}
