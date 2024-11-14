using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM
{
    public class Notions
    {
        public static string StartModeDescription { get; set; } = "<size=30><b><color=#{ModeColor}>{CurrentMode}</color></b></size>\n<size=25>{ModeDescription}</size>\n{CurrentSubMode}<size=15>콘솔(~ 또는 `)을 열고 자세한 설명을 확인해보세요.</size>";
        public static string LateJoinModeDescription { get; set; } = "<size=20>현재 진행중인 모드</size>\n<size=30><b><color=#{ModeColor}>{CurrentMode}</color></b></size>\n<size=25>{ModeDescription}</size>\n{CurrentSubMode}<size=15>콘솔(~ 또는 `)을 열고 자세한 설명을 확인해보세요.</size>";
        public static string WelcomeMessage { get; set; } = "<size=25><b>랜덤게임모드</b>에 오신 것을 환영합니다!</size>";
        public static string LobbyMessage { get; set; } =
"""
<size=120><b>💀 ({Logo}) 💀</b></size>
<size=80><b>[RGM] 랜덤게임모드</b></size> <size=25>(ver. {Version})</size>
<align=right><size=20><b><i><color=#00FA8E>랜</color><color=#13FA96>덤</color><color=#26FA9F>의</color> <color=#4DFAB0>세</color><color=#60FAB9>계</color><color=#73FAC1>에</color> <color=#9AFAD2>빠</color><color=#ADFADB>져</color><color=#C1FAE4>보</color><color=#D4FAEC>세</color><color=#E7FAF5>요</color></i></b></size></align>
<align=left>
Exp: {Exp}
RP: {RP}
<b>Cash: ₩{Cash}</b>

{FirstMark} [1] {First} | {FirstVote}
{SecondMark} [2] {Second} | {SecondVote}
{ThirdMark} [3] {Third} | {ThirdVote}

<color=#{ModeColor}><b>{ModeName}</b></color>
{ModeDescription}{Lines}
<size=25>tip. <i>{Tip}</i></size>
</align>








""";
    }
}
