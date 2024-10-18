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
<align=right><size=20><b><i><color=#FA7000>두</color><color=#DE6302>근</color><color=#C25705>두</color><color=#A64A08>근</color> <color=#6F310D>할</color><color=#532510>로</color><color=#371813>윈</color><color=#1B0C16>!</color></i></b></size></align>
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
