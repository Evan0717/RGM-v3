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
{CurrentSubMode}<size=15>[ESC] -> [Settings] -> [Server-specific]로 이동하여 자세한 설명을 확인해보세요.</size>
""";
        public static string LateJoinModeDescription { get; set; } =
"""
<size=20>현재 진행중인 모드</size>
<size=30><b><color=#{ModeColor}>{CurrentMode}</color></b></size>
<size=25>{ModeDescription}</size> <size=20>({ModeInfo})</size>
{CurrentSubMode}<size=15>[ESC] -> [Settings] -> [Server-specific]로 이동하여 자세한 설명을 확인해보세요.</size>
""";
        public static string WelcomeMessage { get; set; } = "<size=25><b>랜덤게임모드 (RGM)</b>에 오신 것을 환영합니다!</size>";
        public static string LobbyMessage { get; set; } =
"""
<size=120><b><color=#e5dd84>❓</color> ({Logo}) <color=#84e5db>❓</color></b></size>
<size=80><b>[RGM] 랜덤게임모드</b></size> <size=25>(ver. {Version})</size>
<align=right><size=20><b><i>아름다운 세상</i></b></size></align>
<align=left>
Exp: {Exp}
랜덤코인: {RC}
<b>Cash: ₩{Cash}</b>

{FirstMark} [1] {First} | {FirstVote}
{SecondMark} [2] {Second} | {SecondVote}
{ThirdMark} [3] {Third} | {ThirdVote}
{FourthMark} [4] {Fourth} | {FourthVote}

<color=#{ModeColor}><b>{ModeName}</b></color>
{ModeDescription}{Lines}
<size=25>tip. <i>{Tip}</i></size>
</align>








""";
    }
}
