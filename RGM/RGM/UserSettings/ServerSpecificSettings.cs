using Discord;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using MonoMod.Utils;
using RGM.API.Features;
using RGM.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UserSettings.ServerSpecific;
using static RGM.Variables.ServerManagers;
using static System.Net.Mime.MediaTypeNames;

namespace RGM.UserSettings
{
    public static class ServerSpecificSettings
    {
        public static string GetValue(string debugValue)
        {
            try
            {
                return debugValue.Split('(')[1].Split(')')[0].Trim();
            }
            catch 
            {
                return debugValue;
            }
        }

        public static void RegisterCommonSettings(Player player)
        {
            RGMSetting(player);
            UserSetting(player);
            ModeSetting(player);
            // EtcSetting(player);

            Refresh(player);
        }

        public static void UnregisterHeader(string userId, List<string> headerNames = null)
        {
            if (headerNames == null)
            {
                SettingBase.Unregister((p) => { return p.UserId == userId; });
            }
            else
            {
                foreach (var headerName in headerNames)
                {
                    var settingsToRemove = PlayerSettings[userId].Item2.Where(s => s.Header.Label == headerName).ToList();
                    foreach (var settings in settingsToRemove)
                    {
                        PlayerSettings[userId].Item2.Remove(settings);

                        SettingBase.Unregister((p) => { return p.UserId == userId; }, new SettingBase[] { settings.Header });
                        SettingBase.Unregister((p) => { return p.UserId == userId; }, settings.SettingBases);
                    }
                }
            }
        }

        public static void Refresh(Player player)
        {
            if (!PlayerSettings.ContainsKey(player.UserId))
                PlayerSettings[player.UserId] = (new List<SettingBase>(), new List<SettingInfo>());

            PlayerSettings[player.UserId].Item1.Clear();

            var header = new HeaderSetting("-");

            SettingBase.Register(new List<SettingBase> { header }, (p) => { return p == player; });

            SettingBase.Unregister((p) => { return p == player; }, SettingBase.List.Where(x => x != header).ToList());

            var settingsToRemove = PlayerSettings[player.UserId].Item2.ToList();
            foreach (var setting in settingsToRemove)
            {
                setting.Activate(player);
                PlayerSettings[player.UserId].Item2.Remove(setting);
            }

            SettingBase.Register(PlayerSettings[player.UserId].Item1, (p) => { return p == player; });

            SettingBase.Unregister((p) => { return p == player; }, SettingBase.List.Where(x => x == header).ToList());
        }

        public static List<SettingBase> Save(Player player, HeaderSetting header, List<SettingBase> settingBases, Action<Player> action)
        {
            List<SettingBase> sb = settingBases.Where(x => x != null).ToList();

            PlayerSettings[player.UserId].Item1.AddRange(sb);
            PlayerSettings[player.UserId].Item2.Add(new SettingInfo
            {
                SettingBases = sb,
                Header = header,
                Activate = action
            });

            return settingBases;
        }

        public static List<SettingBase> RGMSetting(Player player)
        {
            var header1 = new HeaderSetting("<size=30>[<color=#F78181>K</color><color=#5882FA>R</color>]</size></align> <b><size=30><color=#F6CECE>랜</color><color=#F6D8CE>덤</color><color=#F6E3CE>게</color><color=#F5ECCE>임</color><color=#F5F6CE>모</color><color=#ECF6CE>드</color></size></b>");
            var text1 = new TextInputSetting(1, 
$"""
• <b><color=#F6CECE>랜</color><color=#F6D8CE>덤</color><color=#F6E3CE>게</color><color=#F5ECCE>임</color><color=#F5F6CE>모</color><color=#ECF6CE>드</color></b>는 매 라운드마다 랜덤한 모드와 함께 라운드가 시작되는 한국 서버입니다.
• 콘솔(` 또는 ~)을 열고 .help를 입력하여 사용 가능한 [RGM] 명령어 리스트를 확인할 수 있습니다.

<size=25><align=center><color=#81BEF7><link=https://discord.gg/NWSrVqmKsq>| <b>디스코드 (클릭)</b> |</link></color></align></size>

<size=25><align=center><color=#D0A9F5><link=https://www.randomsl.xyz/rule>| <b>규정 (클릭)</b> |</link></color></align></size>

<size=25><align=center><color=#FA5858><link=https://www.youtube.com/@RandomGameMode>| <b>공식 유튜브 채널 (클릭)</b> |</link></color></align></size>

<size=25><align=center><color=#F7FE2E><link=https://discord.com/channels/930837847026585600/1224668947459084439/1295030194171547779>| <b>상점 (클릭)</b> |</link></color></align></size>

<size=25><align=center><color=#58FAD0><link=https://discord.com/channels/930837847026585600/1295036812791906368/1295040425215791194>| <b>명령어 가이드 (클릭)</b> |</link></color></align></size>
""", header: header1);

            var settingBases = new List<SettingBase> { text1 };

            return Save(player, header1, settingBases, (p) => { RGMSetting(p); });
        }

        public static List<SettingBase> UserSetting(Player player)
        {
            List<string> uc()
            {
                return UsersManager.UsersCache[player.UserId];
            }

            var header1 = new HeaderSetting("<b>ⓘ 유저</b>");
            var text1 = new TextInputSetting(1, $"👤 Steam ID: {player.UserId}<line-height=0>\n</line-height><align=right><mark=#5865f215><link=https://steamcommunity.com/profiles/{player.RawUserId}>ㅤ자신의 스팀 프로필 보기ㅤㅤ</link></mark></align>", header: header1);
            var text2 = new TextInputSetting(2, $"⭐ EXP: {uc()[0]}", header: header1);
            var text3 = new TextInputSetting(3, $"💫 랜덤코인: {uc()[1]}", header: header1);
            var text4 = new TextInputSetting(4, $"💎 Cash: {uc()[2]}<line-height=0>\n</line-height><align=right><mark=#5865f215><link=https://discord.gg/h4AKgks7VMV>ㅤ🏪 Cash 충전하기ㅤㅤ</link></mark></align>", header: header1);
            var text5 = new TextInputSetting(5, uc()[13] == "0" ? $"디스코드에서 아래에 명시된 명령어를 사용하세요.\n'/rgm 연동 <Steam ID> <연동 코드>' (연동 코드: {uc()[14]})" : $"📎 연동된 Discord ID: {uc()[13]}", SSTextArea.FoldoutMode.CollapsedByDefault, hintDescription: uc()[13] == "0" ? $"클릭하여 Discord 연동 코드를 확인하세요." : "✅ Discord와 Steam이 연동된 상태입니다.", header: header1);
            var text6 = new TextInputSetting(6, uc()[3].Split('/').Count() == 0 ? "보유한 킬이펙트가 없습니다." : $"{(uc()[4] == "0" ? "" : $"장착한 킬이펙트: {uc()[4]}\n<size=15>{KillEffects[uc()[4]]}</size>\n")}보유한 킬이펙트\n{string.Join("\n", uc()[3].Split('/').Select(x => $"<size=15>{x}</size>"))}", SSTextArea.FoldoutMode.CollapsedByDefault, hintDescription: $"💀 킬이펙트", header: header1);
            var text7 = new TextInputSetting(7, uc()[19].Split('/').Count() == 0 ? "보유한 스폰이펙트가 없습니다." : $"{(uc()[20] == "0" ? "" : $"장착한 스폰이펙트: {uc()[20]}\n<size=15>{SpawnEffects[uc()[20]]}</size>\n")}보유한 스폰이펙트\n{string.Join("\n", uc()[19].Split('/').Select(x => $"<size=15>{x}</size>"))}", SSTextArea.FoldoutMode.CollapsedByDefault, hintDescription: $"📥 스폰이펙트", header: header1);
            string nick(int num)
            {
                string n = uc()[num];

                if (n == "0")
                    n = "";

                return n;
            }
            var text8 = new TextInputSetting(8, uc()[7].Split('/').Count() == 0 ? "보유한 커스터마이징이 없습니다." : $"{(uc()[7].Split('/').Contains("커스텀 닉네임") ? $"커스텀 닉네임: {uc()[5]}({Tools.CustomFormatter(player, nick(5))})" : "")}{(uc()[7].Split('/').Contains("커스텀 인포") ? $"\n커스텀 인포: {uc()[6]}({Tools.CustomFormatter(player, nick(6))})" : "")}", SSTextArea.FoldoutMode.CollapsedByDefault, hintDescription: $"🔧 커스터마이징", header: header1);
            var text9 = new TextInputSetting(9, uc()[8].Split('/').Count() == 0 ? "보유한 페인트가 없습니다." : $"{(uc()[9] == "0" ? "" : $"장착한 페인트: {uc()[9]}\n<size=15>{Paints[uc()[9]]}</size>\n")}보유한 페인트\n{string.Join("\n", uc()[8].Split('/').Select(x => $"<size=15>{x}</size>"))}", SSTextArea.FoldoutMode.CollapsedByDefault, hintDescription: $"🎨 페인트", header: header1);
            var text10 = new TextInputSetting(11, uc()[10].Split('/').Count() == 0 ? "보유한 칭호가 없습니다." : $"{(uc()[11] == "0" ? "" : $"장착한 칭호: {uc()[11]}\n<size=15>{Badges[uc()[11]]}</size>\n")}보유한 칭호\n{string.Join("\n", uc()[10].Split('/').Select(x => $"<size=15>{x}</size>"))}", SSTextArea.FoldoutMode.CollapsedByDefault, hintDescription: $"🔖 칭호", header: header1);
            TextInputSetting text11 = null;
            if (player.HasReservedSlot)
            {
                text11 = new TextInputSetting(12, $"<b>✨ 풀방 접속권 보유 중 ✨</b>");
                text11.Header = header1;
            }

            var settingBases = new List<SettingBase> { text1, text2, text3, text4, text5, text6, text7, text8, text9, text10, text11 };

            return Save(player, header1, settingBases, (p) => { UserSetting(p); });
        }

        public static List<SettingBase> ModeSetting(Player player)
        {
            var header1 = new HeaderSetting("<b>🎮 모드</b>");
            var text1 = new TextInputSetting(103, "📝 모드 설명\n자세한 설명을 조회할 모드를 선택해주세요.", header: header1);
            IEnumerable<string> modeList = ModeList.Keys.Select(x => $"{x.GetModeData().Name} ({x.GetModeData().Category}, {x.GetModeData().Info})");
            var dropdown1 = new DropdownSetting(102, "📃 전체 모드", modeList, header: header1);
            dropdown1.OnChanged = (p, sb) =>
            {
                string modeName = GetValue(sb.Base.DebugValue);
                ModeType mode = ModeList.Keys.Select(x => x.GetModeData().Name).Contains(modeName) ? (ModeList.Keys.FirstOrDefault(x => x.GetModeData().Name == modeName)).GetModeData().Type : ModeType.Develop;
                text1.Base.SendTextUpdate($"📝 모드 설명\n{(mode.GetModeData().Description)}\n<size=80%>{mode.GetModeData().Detail}</size>");
            };
            var dropdown2 = new DropdownSetting(101, "✅ 활성화된 모드", EnabledModeList.Select(x => $"{x.GetModeData().Name} ({x.GetModeData().Category})"), header: header1);
            dropdown2.OnChanged = (p, sb) =>
            {
                string modeName = GetValue(sb.Base.DebugValue);
                var mode = ModeList.Keys.Select(x => x.GetModeData().Name).Contains(modeName) ? (ModeList.Keys.FirstOrDefault(x => x.GetModeData().Name == modeName)).GetModeData().Type : ModeType.Develop;
                text1.Base.SendTextUpdate($"📝 모드 설명\n{(mode.GetModeData().Description)}\n<size=80%>{mode.GetModeData().Detail}</size>");
            };

            var settingBases = new List<SettingBase> { text1, dropdown1, dropdown2 };

            return Save(player, header1, settingBases, (p) => { ModeSetting(p); });
        }

        public static List<SettingBase> EtcSetting(Player player)
        {
            var header1 = new HeaderSetting("<b>⚙️ 기타</b>");
            var button1 = new ButtonSetting(200, "🔄 새로고침", "모든 정보를 새로고침합니다.", 1, header: header1);
            button1.OnChanged = (p, sb) =>
            {
                try
                {
                    Refresh(player);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    player.SendConsoleMessage(e.ToString(), "red");
                }
            };

            var settingBases = new List<SettingBase> { button1 };

            return Save(player, header1, settingBases, (p) => { EtcSetting(p); });
        }
    }
}
