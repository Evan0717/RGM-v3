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
using UserSettings.ServerSpecific;
using static RGM.Variables.ServerManagers;
using static System.Net.Mime.MediaTypeNames;

namespace RGM.UserSettings
{
    public static class ServerSpecificSettings
    {
        public static void RegisterCommonSettings(Player player)
        {
            RGMSetting(player);
            UserSetting(player);
            ModeSetting(player);
            EtcSetting(player);

            Refresh(player);
        }

        public static void UnregisterSettings(string userId, List<string> headerNames = null)
        {
            if (headerNames == null)
            {
                var settingsToRemove = PlayerSettings[userId].Item2.ToList();
                foreach (var settings in settingsToRemove)
                {
                    PlayerSettings[userId].Item2.Remove(settings);

                    SettingBase.Unregister((p) => { return p.UserId == userId; }, settings.SettingBases);
                }
            }
            else
            {
                foreach (var headerName in headerNames)
                {
                    var settingsToRemove = PlayerSettings[userId].Item2.Where(s => s.Header.Label == headerName).ToList();
                    foreach (var settings in settingsToRemove)
                    {
                        PlayerSettings[userId].Item2.Remove(settings);

                        SettingBase.Unregister((p) => { return p.UserId == userId; }, settings.SettingBases);
                    }
                }
            }
        }

        public static void Refresh(Player player)
        {
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
            PlayerSettings[player.UserId].Item1.AddRange(settingBases);
            PlayerSettings[player.UserId].Item2.Add(new SettingInfo
            {
                SettingBases = settingBases,
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
""", header: header1);

            var settingBases = new List<SettingBase> { text1 };

            return Save(player, header1, settingBases, (p) => { RGMSetting(p); });
        }

        public static List<SettingBase> UserSetting(Player player)
        {
            var userCache = UsersManager.UsersCache[player.UserId];

            var header1 = new HeaderSetting("<b>ⓘ 유저 정보</b>");
            var text1 = new TextInputSetting(1, $"👤 유저 ID: {player.UserId}<line-height=0>\n</line-height><align=right><mark=#5865f215><link=https://steamcommunity.com/profiles/{player.RawUserId}>ㅤ자신의 스팀 프로필 보기ㅤㅤ</link></mark></align>", header: header1);
            var text2 = new TextInputSetting(2, $"⭐ EXP: {userCache[0]}", header: header1);
            var text3 = new TextInputSetting(3, $"💫 RP: {userCache[1]}", header: header1);
            var text4 = new TextInputSetting(4, $"💎 Cash: {userCache[2]}<line-height=0>\n</line-height><align=right><mark=#5865f215><link=https://discord.gg/h4AKgks7VMV>ㅤ🏪 Cash 충전하기ㅤㅤ</link></mark></align>", header: header1);

            var settingBases = new List<SettingBase> { text1, text2, text3, text4 };

            return Save(player, header1, settingBases, (p) => { UserSetting(p); });
        }

        public static List<SettingBase> ModeSetting(Player player)
        {
            var header1 = new HeaderSetting("<b>🎮 모드</b>");
            var text1 = new TextInputSetting(103, "<align=left><size=105%>📝 모드 설명</align></size>\n자세한 설명을 조회할 모드를 선택해주세요.", header: header1);
            var dropdown1 = new DropdownSetting(101, "✅ 활성화된 모드", EnabledModeList.Select(x => $"{x.GetModeData().Name} ({x.GetModeData().Category})"), header: header1, onChanged: (p, sb) =>
            {
                string modeName = sb.Base.DebugValue.Split('(')[1].Trim();
                var mode = ModeList.Keys.Select(x => x.GetModeData().Name).Contains(modeName) ? (ModeList.Keys.FirstOrDefault(x => x.GetModeData().Name == modeName)).GetModeData().Type : ModeType.Develop;
                text1.Base.SendTextUpdate($"<align=left><size=105%>📝 모드 설명</align></size>\n{(mode.GetModeData().Description)}\n<size=80%>{mode.GetModeData().Detail}</size>");
            });
            var dropdown2 = new DropdownSetting(102, "📃 전체 모드", ModeList.Keys.Select(x => $"{x.GetModeData().Name} ({x.GetModeData().Category}, {x.GetModeData().Info})"), header: header1, onChanged: (p, sb) =>
            {
                string modeName = sb.Base.DebugValue.Split('(')[1].Trim();
                var mode = ModeList.Keys.Select(x => x.GetModeData().Name).Contains(modeName) ? (ModeList.Keys.FirstOrDefault(x => x.GetModeData().Name == modeName)).GetModeData().Type : ModeType.Develop;
                text1.Base.SendTextUpdate($"<align=left><size=105%>📝 모드 설명</align></size>\n{(mode.GetModeData().Description)}\n<size=80%>{mode.GetModeData().Detail}</size>");
            });

            var settingBases = new List<SettingBase> { text1, dropdown1, dropdown2 };

            return Save(player, header1, settingBases, (p) => { ModeSetting(p); });
        }

        public static List<SettingBase> EtcSetting(Player player)
        {
            var header1 = new HeaderSetting("<b>⚙️ 기타</b>");
            var button1 = new ButtonSetting(200, "🔄 새로고침", "모든 정보를 새로고침합니다.", 1, header: header1, onChanged: (p, sb) =>
            {
                Refresh(player);
            });

            var settingBases = new List<SettingBase> { button1 };

            return Save(player, header1, settingBases, (p) => { EtcSetting(p); });
        }
    }
}
