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

namespace RGM.UserSettings
{
    public static class ServerSpecificSettings
    {
        public static HeaderSetting RGM { get; private set; } = new HeaderSetting(429022, "<size=30>[<color=#F78181>K</color><color=#5882FA>R</color>]</size></align> <b><size=30><color=#F6CECE>랜</color><color=#F6D8CE>덤</color><color=#F6E3CE>게</color><color=#F5ECCE>임</color><color=#F5F6CE>모</color><color=#ECF6CE>드</color></size></b>");

        public static HeaderSetting Setting { get; private set; } = new HeaderSetting(19287, "<b>⚙️ 설정</b>");
        public static KeybindSetting ScpCanEquipRandomItem { get; private set; }
        public static ButtonSetting SpectatorToNone { get; private set; }
        public static ButtonSetting SwitchToSpectator { get; private set; }
        public static TwoButtonsSetting MuteBGM { get; private set; }
        public static DropdownSetting Translation { get; private set; }

        public static void Init()
        {
            ScpCanEquipRandomItem = new KeybindSetting(
                id: 12050,
                label: "SCP의 아이템 장착ㅣEquipping SCP items",
                suggested: KeyCode.H,
                hintDescription:
"""
SCP가 보유한 아이템 중 무작위로 하나를 장착합니다.

Equip a random item from the SCP's inventory.
""",
                header: Setting,
                allowSpectatorTrigger: false
            );

            SpectatorToNone = new ButtonSetting(
                id: 12051,
                label: "관전석 <-> 훈련장ㅣSpectator <-> Training ground",
                buttonText: "GO!",
                hintDescription:
"""
관전석에서 훈련장으로 이동합니다.
• Set 모드 또는 특정 모드에서 사용 불가
• 사망 후 10초가 지나야 사용 가능

Move from the spectator seats to the training grounds.
• Not available in Set mode or certain modes.
• Available 10 seconds after death.
""",
                
                header: Setting,
                holdTime: 0.5f
            );

            SwitchToSpectator = new ButtonSetting(
                id: 12052,
                label: "관전자 <-> 오버워치ㅣSpectator <-> Overwatch",
                buttonText: "<->",
                hintDescription:
"""
관전자와 오버워치 상태를 변경합니다.
• 사망 후 10초가 지나야 사용 가능

Changes between spectator and Overwatch status.
• Available 10 seconds after death.
""",
                header: Setting,
                holdTime: 0.5f
            );

            MuteBGM = new TwoButtonsSetting(
                id: 12053,
                label: "BGM 음소거ㅣBGM mute",
                firstOption: "ON",
                secondOption: "OFF",
                defaultIsSecond: true,
                hintDescription:
"""
음악이 유튜브 저작권에 걸릴 것 같다고요? 이 기능을 사용하세요.

Are you worried BGM might be copyrighted by YouTube? Use this feature.
""",
                header: Setting
            );

            Translation = new DropdownSetting(
                id: 12054,
                label: "번역ㅣTranslation",
                options: new string[]
                {
                    "Chinese (zh)",
                    "English (en)",
                    "German (de)",
                    "Italian (it)",
                    "Japanese (ja)",
                    "Korean (ko)",
                    "Russian (ru)",
                },
                defaultOptionIndex: 5,
                hintDescription: 
"""
언어의 장벽을 부수려면 이 설정을 사용하세요.

Use this setting to break the language barrier.
""",
                header: Setting
            );

            IEnumerable<SettingBase> settings = new SettingBase[]
            {
                // 설정
                ScpCanEquipRandomItem, 
                SpectatorToNone, 
                SwitchToSpectator,
                MuteBGM,
                Translation,
            };

            SettingBase.Register(settings);
        }

        public static void OnSSInput(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            Player player = Player.Get(sender);

            // 키바인드인 경우
            if (setting is SSKeybindSetting keybind && keybind.SyncIsPressed)
            {
                if (setting.SettingId == 12050)
                {
                    if (player.IsScpRole())
                    {
                        var candidates = player.Items
                            .Where(x => player.CurrentItem != x)
                            .ToList();

                        candidates.Add(null);

                        if (candidates.Count == 0)
                            return;

                        player.CurrentItem = candidates.GetRandomValue();
                        return;
                    }
                }
            }

            // 투버튼인 경우
            if (setting is SSTwoButtonsSetting twoButton)
            {
                if (setting.SettingId == 12053)
                {
                    if (twoButton.SyncIsA)
                    {
                        if (!MuteBGMPlayers.Contains(player))
                            MuteBGMPlayers.Add(player);
                    }
                    else
                    {
                        if (MuteBGMPlayers.Contains(player))
                            MuteBGMPlayers.Remove(player);
                    }
                }
            }

            // 드롭다운인 경우
            if (setting is SSDropdownSetting dropdown)
            {
                if (setting.SettingId == 12054)
                {
                    TranslatorPlayers[player] = dropdown.SyncSelectionText.Split('(')[1].Replace(")", "");
                }
            }

            if (setting.SettingId == 12051)
            {
                if ((CurrentMode == ModeType.None || CurrentMode.GetModeData().Info == ModeInfo.Plus) && 
                    IsNonePlayerAllowed &&
                    (Round.IsLobby || (DateTime.UtcNow - PlayersReport[player.UserId].LastDeath).TotalSeconds >= 10))
                {
                    if (player.IsAlive && NonePlayer.Players.Contains(player))
                    {
                        player.ClearInventory();
                        player.Kill("관전석으로 되돌아갑니다.");
                    }
                    else if (Round.IsLobby ? true : player.IsDead)
                    {
                        NonePlayer.Create(player);
                    }
                    else
                    {
                        PlayersAudio[player].TryPlay($"nope");
                    }
                }
                else
                {
                    PlayersAudio[player].TryPlay($"nope");
                }
            }

            if (setting.SettingId == 12052)
            {
                if ((DateTime.UtcNow - PlayersReport[player.UserId].LastDeath).TotalSeconds >= 10)
                {
                    if (player.Role.Type == RoleTypeId.Overwatch)
                    {
                        player.Role.Set(RoleTypeId.Spectator);
                    }
                    else if (player.Role.Type == RoleTypeId.Spectator)
                    {
                        player.Role.Set(RoleTypeId.Overwatch);
                    }
                    else
                    {
                        PlayersAudio[player].TryPlay($"nope");
                    }
                }
                else
                {
                    PlayersAudio[player].TryPlay($"nope");
                }
            }
        }
    }
}
