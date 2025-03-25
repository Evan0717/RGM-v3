using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserSettings.ServerSpecific;
using static RGM.Variables.ServerManagers;

namespace RGM.UserSettings
{
    public static class SSSBase
    {
        public static void Refresh(Player player)
        {
            if (!SSSBases.ContainsKey(player))
                SSSBases.Add(player, new());

            ServerSpecificSettingsSync.SendToPlayer(player.ReferenceHub, SSSBases[player].ToArray());
        }

        public static void SetModeSettings(Player player)
        {
            HeaderSetting header1 = new("🎮 모드", "모드에 관련된 설정 모음입니다.");
            DropdownSetting drop1 = new(1, "활성화된 모드", EnabledModeList.Select(x => $"{x.GetModeData().Name} ({x.GetModeData().Category}, {x.GetModeData().Info})"), header: header1);
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += (rh, sss) =>
            {
                if (sss.Label == "")
                {

                }
            };

            Refresh(player);
        }
    }
}
