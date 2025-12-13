using Discord;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using MonoMod.Utils;
using MultiBroadcast.API;
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
using UnityEngine;
using UserSettings.ServerSpecific;
using static RGM.Variables.ServerManagers;
using static System.Net.Mime.MediaTypeNames;

namespace RGM.UserSettings
{
    public static class ServerSpecificSettings
    {
        public static HeaderSetting RGM { get; private set; } = new HeaderSetting(19287, "[RGM] 랜덤게임모드");
        public static KeybindSetting ScpCanEquipRandomItem { get; private set; }

        public static void RegisterSettings()
        {
            ScpCanEquipRandomItem = new KeybindSetting(
                id: 12050,
                label: "아이템 장착",
                suggested: KeyCode.H,
                hintDescription: "SCP가 보유한 아이템 중 무작위로 하나를 장착합니다.",
                header: RGM,
                allowSpectatorTrigger: false
            );

            SettingBase.Register(new[] { ScpCanEquipRandomItem });
        }

        public static void OnSSInput(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            if (setting is SSKeybindSetting keybind)
            {
                if (keybind.SyncIsPressed)
                {
                    Player player = Player.Get(sender);

                    if (setting.SettingId == 12050)
                    {
                        if (player.IsScp)
                        {
                            var candidates = player.Items
                                .Where(x => player.CurrentItem != x)
                                .ToList();

                            if (candidates.Count == 0)
                                return;

                            var index = UnityEngine.Random.Range(0, candidates.Count);
                            player.CurrentItem = candidates[index];
                            return;
                        }
                    }
                }
            }
        }
    }
}
