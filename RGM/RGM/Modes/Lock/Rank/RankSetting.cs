using Discord;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using HarmonyLib;
using MEC;
using MonoMod.Utils;

using PlayerRoles;
using RGM.API.Features;
using RGM.API.Interfaces;
using RGM.Commands.ClientCommands;
using RGM.Modes.SubClass;
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
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    public static class RankSetting
    {
        public static HeaderSetting RankHeader { get; private set; } = new HeaderSetting(1900, "");

        public static DropdownSetting ClassDSetting { get; private set; }

        public static void Init()
        {
            ClassDSetting = new DropdownSetting(
                id: 12050,
                label: "SCP의 아이템 장착ㅣEquipping SCP items",
                suggested: KeyCode.H,
                hintDescription:
"""
""",
                header: RankHeader,
                allowSpectatorTrigger: false
            );

            IEnumerable<SettingBase> settings = new SettingBase[]
            {
                // 설정
                ClassDSetting
            };

            SettingBase.Register(settings);
        }

        public static void OnSSInput(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            Player player = Player.Get(sender);

            // 드롭다운인 경우
            if (setting is SSDropdownSetting dropdown)
            {
                if (setting.SettingId == 12054)
                {
                    TranslatorPlayers[player] = dropdown.SyncSelectionText.Split('(')[1].Replace(")", "");
                }
            }
        }
    }
}
