using Discord;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using MEC;
using MonoMod.Utils;
using MultiBroadcast.API;
using PlayerRoles;
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
        public static ButtonSetting SpectatorToNone { get; private set; }
        public static string SpectatorToNone_Text = "사망자 <-> 훈련장";
        public static ButtonSetting SwitchToSpectator { get; private set; }

        public static void RegisterSettings()
        {
            ScpCanEquipRandomItem = new KeybindSetting(
                id: 12050,
                label: "SCP의 아이템 장착",
                suggested: KeyCode.H,
                hintDescription: "SCP가 보유한 아이템 중 무작위로 하나를 장착합니다.",
                header: RGM,
                allowSpectatorTrigger: false
            );

            SpectatorToNone = new ButtonSetting(
                id: 12051,
                label: SpectatorToNone_Text,
                buttonText: "꾹 눌러주세요 ❤️",
                hintDescription:
"""
관전석에서 훈련장으로 이동합니다. 사망자는 "관전자"와 "오버워치" 둘 다 포함합니다.

• Set 모드에서 사용 불가
• 사망 후 10초가 지나야 사용 가능
""",
                
                header: RGM,
                holdTime: 0.5f
            );

            SwitchToSpectator = new ButtonSetting(
                id: 12052,
                label: "관전자 <-> 오버워치",
                buttonText: "꾹꾹 ❤️❤️",
                hintDescription: "관전자와 오버워치 상태를 변경합니다.",
                header: RGM,
                holdTime: 0.5f
            );

            IEnumerable<SettingBase> settings = new SettingBase[]
            {
                ScpCanEquipRandomItem, 
                SpectatorToNone, 
                SwitchToSpectator
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
                if (Round.IsStarted && CurrentMode.GetModeData().Info != ModeInfo.Set && 
                    (DateTime.UtcNow - PlayersReport[player.UserId].LastDeath).TotalSeconds >= 10)
                {
                    if (player.IsAlive && NonePlayers.Contains(player))
                    {
                        player.ClearInventory();
                        player.Role.Set(RoleTypeId.Spectator);
                    }
                    else if (player.IsDead)
                    {
                        IEnumerator<float> none()
                        {
                            if (!NonePlayers.Contains(player))
                                NonePlayers.Add(player);

                            player.Role.Set(RoleTypeId.Tutorial);
                            player.Position = new Vector3(20.16966f, 275.0556f, -29.42459f);
                            player.AddItem(Tools.EnumToList<ItemType>().GetRandomValue(x => x.IsWeapon()));
                            player.AddItem(Tools.EnumToList<ItemType>().GetRandomValue());

                            while (player.Role.Type == RoleTypeId.Tutorial)
                            {
                                yield return Timing.WaitForOneFrame;
                            }

                            if (NonePlayers.Contains(player))
                                NonePlayers.Remove(player);
                        }

                        Timing.RunCoroutine(none());
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
        }
    }
}
