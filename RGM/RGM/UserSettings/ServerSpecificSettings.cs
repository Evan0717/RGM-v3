using Discord;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using MonoMod.Utils;
using RGM.API.Features;
using RGM.API.Interfaces;
using System;
using System.Collections.Generic;
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
            EtcSetting(player);

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
• <b>🏪 상점</b>에서 다양한 아이템을 구매해보세요!

<size=25><align=center><color=#81BEF7><link=https://discord.gg/NWSrVqmKsq>| <b>디스코드 (클릭)</b> |</link></color></align></size>

<size=25><align=center><color=#D0A9F5><link=https://www.randomsl.xyz/rule>| <b>규정 (클릭)</b> |</link></color></align></size>

<size=25><align=center><color=#FA5858><link=https://www.youtube.com/@RandomGameMode>| <b>공식 유튜브 채널 (클릭)</b> |</link></color></align></size>
""", foldoutMode: SSTextArea.FoldoutMode.CollapsedByDefault, header: header1);

            var settingBases = new List<SettingBase> { text1 };

            return Save(player, header1, settingBases, (p) => { RGMSetting(p); });
        }

        public static List<SettingBase> UserSetting(Player player)
        {
            var uc = UsersManager.UsersCache[player.UserId];

            var header1 = new HeaderSetting("<b>ⓘ 유저</b>");
            var text1 = new TextInputSetting(1, $"👤 유저 ID: {player.UserId}<line-height=0>\n</line-height><align=right><mark=#5865f215><link=https://steamcommunity.com/profiles/{player.RawUserId}>ㅤ자신의 스팀 프로필 보기ㅤㅤ</link></mark></align>", header: header1);
            var text2 = new TextInputSetting(2, $"⭐ EXP: {uc[0]}", header: header1);
            var text3 = new TextInputSetting(3, $"💫 RP: {uc[1]}", header: header1);
            var text4 = new TextInputSetting(4, $"💎 Cash: {uc[2]}<line-height=0>\n</line-height><align=right><mark=#5865f215><link=https://discord.gg/h4AKgks7VMV>ㅤ🏪 Cash 충전하기ㅤㅤ</link></mark></align>", header: header1);
            DropdownSetting dropdown1 = null;
            if (uc[3] != "0")
            {
                dropdown1 = new DropdownSetting(5, "💀 킬이펙트", uc[3].Split('/').Append("-"), header: header1, onChanged: (p, sb) =>
                {
                    string value = GetValue(sb.Base.DebugValue);

                    if (value == "-")
                        value = "0";

                    uc[4] = value;
                    UsersManager.UsersCache[player.UserId] = uc;
                    UsersManager.SaveUsers();
                });
                dropdown1.DefaultOption = uc[4] == "0" ? "-" : uc[4];
            }
            UserTextInputSetting text5 = null;
            if (uc[7].Split('/').Contains("커스텀 닉네임"))
            {
                string nick()
                {
                    string n = uc[5];

                    if (n == "0")
                        n = "";

                    return n;
                }

                text5 = new UserTextInputSetting(6, $"📘 커스텀 닉네임 [{Tools.CustomFormatter(player, nick())}]", nick());
                text5.Header = header1;
                text5.OnChanged = (p, sb) =>
                {
                    string value = GetValue(sb.Base.DebugValue);

                    if (value == "")
                        value = "0";

                    uc[5] = value;
                    UsersManager.UsersCache[player.UserId] = uc;
                    UsersManager.SaveUsers();

                    text5.Label = $"📘 커스텀 닉네임 [{Tools.CustomFormatter(player, nick())}]";
                };
            }
            UserTextInputSetting text6 = null;
            if (uc[7].Split('/').Contains("커스텀 인포"))
            {
                string info()
                {
                    string n = uc[6];

                    if (n == "0")
                        n = "";

                    return n;
                }

                text6 = new UserTextInputSetting(7, $"📙 커스텀 인포 [{Tools.CustomFormatter(player, info())}]", info());
                text6.Header = header1;
                text6.OnChanged = (p, sb) =>
                {
                    string value = GetValue(sb.Base.DebugValue);

                    if (value == "")
                        value = "0";

                    uc[6] = value;
                    UsersManager.UsersCache[player.UserId] = uc;
                    UsersManager.SaveUsers();

                    text6.Label = $"📙 커스텀 인포 [{Tools.CustomFormatter(player, info())}]";
                };
            }
            DropdownSetting dropdown2 = null;
            if (uc[8] != "0")
            {
                dropdown2 = new DropdownSetting(8, "🎨 페인트", uc[8].Split('/').Append("-"), header: header1, onChanged: (p, sb) =>
                {
                    string value = GetValue(sb.Base.DebugValue);

                    if (value == "-")
                        value = "0";

                    uc[9] = value;
                    UsersManager.UsersCache[player.UserId] = uc;
                    UsersManager.SaveUsers();

                    try { Tools.RemovePaint(player); } catch { }

                    try
                    {
                        if (value != "0")
                            Tools.ChangePaint(player, uc[9]);
                    }
                    catch
                    {
                    }
                });
                dropdown2.DefaultOption = uc[9] == "0" ? "-" : uc[9];
            }
            DropdownSetting dropdown3 = null;
            if (uc[10] != "0")
            {
                dropdown3 = new DropdownSetting(9, "🔖 칭호", uc[10].Split('/').Append("-"), header: header1, onChanged: (p, sb) =>
                {
                    string value = GetValue(sb.Base.DebugValue);

                    if (value == "-")
                        value = "0";

                    uc[11] = value;
                    UsersManager.UsersCache[player.UserId] = uc;
                    UsersManager.SaveUsers();

                    if (value != "0")
                        player.RankName = $"{(BadgeIcons.ContainsKey(uc[11]) ? $"{BadgeIcons[uc[11]]} " : "")}{uc[11]}";

                    else
                        player.RankName = null;
                });
                dropdown3.DefaultOption = uc[11] == "0" ? "-" : uc[11];
            }
            TextInputSetting text7 = null;
            if (player.HasReservedSlot)
            {
                text7 = new TextInputSetting(10, $"<b>✨ 풀방 접속권 보유 중 ✨</b>", header: header1);
            }

            var settingBases = new List<SettingBase> { text1, text2, text3, text4, dropdown1, text5, text6, dropdown2, dropdown3, text7 };

            return Save(player, header1, settingBases, (p) => { UserSetting(p); });
        }

        public static List<SettingBase> ModeSetting(Player player)
        {
            var header1 = new HeaderSetting("<b>🎮 모드</b>");
            var text1 = new TextInputSetting(103, "📝 모드 설명\n자세한 설명을 조회할 모드를 선택해주세요.", header: header1);
            IEnumerable<string> modeList = ModeList.Keys.Select(x => $"{x.GetModeData().Name} ({x.GetModeData().Category}, {x.GetModeData().Info})");
            var dropdown1 = new DropdownSetting(102, "📃 전체 모드", modeList, header: header1, onChanged: (p, sb) =>
            {
                string modeName = GetValue(sb.Base.DebugValue);
                ModeType mode = ModeList.Keys.Select(x => x.GetModeData().Name).Contains(modeName) ? (ModeList.Keys.FirstOrDefault(x => x.GetModeData().Name == modeName)).GetModeData().Type : ModeType.Develop;
                text1.Base.SendTextUpdate($"📝 모드 설명\n{(mode.GetModeData().Description)}\n<size=80%>{mode.GetModeData().Detail}</size>");
            });
            var dropdown2 = new DropdownSetting(101, "✅ 활성화된 모드", EnabledModeList.Select(x => $"{x.GetModeData().Name} ({x.GetModeData().Category})"), header: header1, onChanged: (p, sb) =>
            {
                string modeName = GetValue(sb.Base.DebugValue);
                var mode = ModeList.Keys.Select(x => x.GetModeData().Name).Contains(modeName) ? (ModeList.Keys.FirstOrDefault(x => x.GetModeData().Name == modeName)).GetModeData().Type : ModeType.Develop;
                text1.Base.SendTextUpdate($"📝 모드 설명\n{(mode.GetModeData().Description)}\n<size=80%>{mode.GetModeData().Detail}</size>");
            });

            var settingBases = new List<SettingBase> { text1, dropdown1, dropdown2 };

            return Save(player, header1, settingBases, (p) => { ModeSetting(p); });
        }

        public static List<SettingBase> EtcSetting(Player player)
        {
            var header1 = new HeaderSetting("<b>⚙️ 기타</b>");
            var button1 = new ButtonSetting(200, "🔄 새로고침", "모든 정보를 새로고침합니다.", 1, header: header1);
            button1.OnChanged = (p, sb) =>
            {
                Refresh(player);
            };

            var settingBases = new List<SettingBase> { button1 };

            return Save(player, header1, settingBases, (p) => { EtcSetting(p); });
        }
    }
}
