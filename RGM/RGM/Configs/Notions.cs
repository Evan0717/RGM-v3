using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM
{
    public class Notions
    {
        public static string StartModeDescription { get; set; } = 
"""
<size=30><b><color=#{ModeColor}>{CurrentMode}</color></b></size>
<size=25>{ModeDescription}</size> <size=20>({ModeInfo})</size>
{CurrentSubMode}<size=15>콘솔(~ 또는 `)을 열고 자세한 설명을 확인해보세요.</size>
""";
        public static string LateJoinModeDescription { get; set; } =
"""
<size=20>현재 진행중인 모드</size>
<size=30><b><color=#{ModeColor}>{CurrentMode}</color></b></size>
<size=25>{ModeDescription}</size> <size=20>({ModeInfo})</size>
{CurrentSubMode}<size=15>콘솔(~ 또는 `)을 열고 자세한 설명을 확인해보세요.</size>
""";
        public static string WelcomeMessage { get; set; } = "<size=25><b>랜덤게임모드 (RGM)</b>에 오신 것을 환영합니다!</size>";
        public static string LobbyMessage { get; set; } =
"""
<size=120><b><color=red>🎄</color> ({Logo}) <color=#31B404>🎄</color></b></size>
<size=80><b>[RGM] 랜덤게임모드</b></size> <size=25>(ver. {Version})</size>
<align=right><size=20><b><i><color=#FE8686>연</color><color=#FE868B>말</color><color=#FE8691>을</color> <color=#FE879C>맞</color><color=#FE88A2>이</color><color=#FE88A7>하</color><color=#FE89AD>세</color><color=#FE89B3>요</color></i></b></size></align>
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
