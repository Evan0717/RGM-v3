using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UserSettings.ServerSpecific;
using static DAONTFT.Core.Variables.Base;

namespace DAONTFT.Core.TFT
{
    public static class TFTSetting
    {
        public static HeaderSetting RankHeader { get; private set; } = new HeaderSetting(190023, "전략적 팀 전투");

        public static KeybindSetting TFTInfoKey { get; private set; }

        public static void Init()
        {
            List<SettingBase> list = new();

            TFTInfoKey = new KeybindSetting(
               id: 28278,
               label: "<b>전략적 팀 전투 정보 키</b>",
               suggested: KeyCode.F1,
               preventInteractionOnGUI: true,
               hintDescription: "자신의 정보를 볼 때 누르는 키입니다.",
               header: RankHeader
               );
            SettingBase[] settings = new SettingBase[]
            {
                TFTInfoKey,
            };

            foreach (var setting in settings)
                list.Add(setting);

            SettingBase.Register(list);
        }

        public static void OnSSInput(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            Player player = Player.Get(sender);

            if (setting is SSKeybindSetting keybind) 
            {
                if (keybind.SyncIsPressed)
                {
                    if (keybind.SettingId == 28278)
                    {
                        if (!PlayerShowTFTs.ContainsKey(player))
                            PlayerShowTFTs.Add(player, false);

                        if (PlayerShowTFTs[player])
                            PlayerShowTFTs[player] = false;
                        else
                            PlayerShowTFTs[player] = true;

                    }
                }
            }
        }
    }
}
