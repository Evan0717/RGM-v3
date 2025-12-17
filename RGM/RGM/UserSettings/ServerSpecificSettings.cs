using Discord;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using HarmonyLib;
using MEC;
using MonoMod.Utils;
using MultiBroadcast.API;
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
        public static TextInputSetting Description { get; private set; }

        public static HeaderSetting Setting { get; private set; } = new HeaderSetting(19287, "<b>⚙️ 설정</b>");
        public static KeybindSetting ScpCanEquipRandomItem { get; private set; }
        public static ButtonSetting SpectatorToNone { get; private set; }
        public static ButtonSetting SwitchToSpectator { get; private set; }

        public static HeaderSetting Info { get; private set; } = new HeaderSetting(2111, "<b>ⓘ 유저</b>");
        public static TextInputSetting SteamProfile { get; private set; }
        public static TextInputSetting Exp { get; private set; }
        public static TextInputSetting RandomCoin { get; private set; }
        public static TextInputSetting Cash { get; private set; }
        public static TextInputSetting LinkToDiscord { get; private set; }
        public static TextInputSetting KillEffects { get; private set; }
        public static TextInputSetting SpawnEffects { get; private set; }
        public static TextInputSetting Customs { get; private set; }
        public static TextInputSetting Paints { get; private set; }
        public static TextInputSetting Badges { get; private set; }
        public static TextInputSetting ReservedSlot { get; private set; }

        public static HeaderSetting Mode { get; private set; } = new HeaderSetting(3001, "<b>🎮 모드</b>");
        public static TextInputSetting ModeDescription { get; private set; }
        public static DropdownSetting AllModes { get; private set; }
        public static DropdownSetting CurrentModes { get; private set; }

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

        public static void Init()
        {
            Description = new TextInputSetting(29, 
$"""
• <b><color=#F6CECE>랜</color><color=#F6D8CE>덤</color><color=#F6E3CE>게</color><color=#F5ECCE>임</color><color=#F5F6CE>모</color><color=#ECF6CE>드</color></b>는 매 라운드마다 랜덤한 모드와 함께 라운드가 시작되는 한국 서버입니다.
• 콘솔(` 또는 ~)을 열고 .help를 입력하여 사용 가능한 [RGM] 명령어 리스트를 확인할 수 있습니다.

<size=25><align=center><color=#81BEF7><link=https://discord.gg/NWSrVqmKsq>| <b>디스코드 (클릭)</b> |</link></color></align></size>

<size=25><align=center><color=#D0A9F5><link=https://www.randomsl.xyz/rule>| <b>규정 (클릭)</b> |</link></color></align></size>

<size=25><align=center><color=#FFFFFF><link=https://www.youtube.com/@RandomGameMode>| <b>공식 유튜브 채널 (클릭)</b> |</link></color></align></size>

<size=25><align=center><color=#FA5858><link=https://www.youtube.com/@GoldenPig1205>| <b>개발자 유튜브 채널 (클릭)</b> |</link></color></align></size>

<size=25><align=center><color=#89d953><link=https://chzzk.naver.com/acb253a537e8a02632532a8f27fafcaa>| <b>개발자 치지직 채널 (클릭)</b> |</link></color></align></size>
""", header: RGM);

            ScpCanEquipRandomItem = new KeybindSetting(
                id: 12050,
                label: "SCP의 아이템 장착",
                suggested: KeyCode.H,
                hintDescription: "SCP가 보유한 아이템 중 무작위로 하나를 장착합니다.",
                header: Setting,
                allowSpectatorTrigger: false
            );

            SpectatorToNone = new ButtonSetting(
                id: 12051,
                label: "관전석 <-> 훈련장",
                buttonText: "꾹 눌러주세요 ❤️",
                hintDescription:
"""
관전석에서 훈련장으로 이동합니다.

• Set 모드 또는 특정 모드에서 사용 불가
• 사망 후 30초가 지나야 사용 가능
""",
                
                header: Setting,
                holdTime: 0.5f
            );

            SwitchToSpectator = new ButtonSetting(
                id: 12052,
                label: "관전자 <-> 오버워치",
                buttonText: "꾹꾹 ❤️❤️",
                hintDescription:
"""
관전자와 오버워치 상태를 변경합니다.

• 사망 후 10초가 지나야 사용 가능
""",
                header: Setting,
                holdTime: 0.5f
            );

            IEnumerable<SettingBase> settings = new SettingBase[]
            {
                // 설명
                Description,

                // 설정
                ScpCanEquipRandomItem, 
                SpectatorToNone, 
                SwitchToSpectator,
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

            if (setting.SettingId == 12051)
            {
                if (Round.IsStarted && 
                    CurrentMode.GetModeData().Info != ModeInfo.Set && 
                    IsNonePlayerAllowed &&
                    (DateTime.UtcNow - PlayersReport[player.UserId].LastDeath).TotalSeconds >= 10)
                {
                    if (player.IsAlive && NonePlayer.Players.Contains(player))
                    {
                        player.ClearInventory();
                        player.Kill("관전석으로 되돌아갑니다.");
                    }
                    else if (player.IsDead)
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
