using Discord;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using HarmonyLib;
using MEC;
using MonoMod.Utils;

using PlayerRoles;
using RGM.API.DataBases;
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
        private const int RankSettingIdStart = 2044;
        private static readonly Dictionary<int, (RankAbilityCategory AbilityCategory, RankCategory? RankCategory)> RankDropdownMeta = new();

        public static HeaderSetting RankHeader { get; private set; } = new HeaderSetting(190023, "경쟁전");

        public static KeybindSetting GadgetKey { get; private set; }

        public static void Init()
        {
            GadgetKey = new KeybindSetting(
                id: 289289923, 
                label: "경쟁전 가젯 키", 
                suggested: KeyCode.Z, 
                preventInteractionOnGUI: true, 
                hintDescription: "경쟁전에서 가젯을 사용할 때 누르는 키입니다.", 
                header: RankHeader
                );

            // ---------------------------------------------------------------------------------------------------------------------------------------------

            List<DropdownSetting> dropdowns = new();
            RankDropdownMeta.Clear();
            int nextSettingId = RankSettingIdStart;

            var categories = RankInfo.변칙성.Keys.Union(RankInfo.가젯.Keys).Distinct();

            foreach (var category in categories)
            {
                string roleName = category.ToString().Contains("SCP") ? category.ToString().Replace("_", "-") : category.ToString().Replace("_", " ");

                if (RankInfo.변칙성.TryGetValue(category, out var anomalyDict))
                {
                    int settingId = nextSettingId++;
                    dropdowns.Add(new DropdownSetting(
                        id: settingId,
                        label: $"<color={RankAbilityCategoryExtensions.GetColor(RankAbilityCategory.변칙성)}>변칙성</color> <color={category.GetRoleCategory().GetColor().ToHex()}>{roleName}</color>",
                        hintDescription: string.Join("\n", anomalyDict.Select(x => $"• {x.Key}: {x.Value.Item1}")),
                        options: anomalyDict.Keys,
                        header: RankHeader
                    ));
                    RankDropdownMeta[settingId] = (RankAbilityCategory.변칙성, category);
                }

                if (RankInfo.가젯.TryGetValue(category, out var gadgetDict))
                {
                    int settingId = nextSettingId++;
                    dropdowns.Add(new DropdownSetting(
                        id: settingId,
                        label: $"<color={RankAbilityCategoryExtensions.GetColor(RankAbilityCategory.가젯)}>가젯</color> <color={category.GetRoleCategory().GetColor().ToHex()}>{roleName}</color>",
                        hintDescription: string.Join("\n", gadgetDict.Select(x => $"• {x.Key}: {x.Value.Item1}")),
                        options: gadgetDict.Keys,
                        header: RankHeader
                    ));
                    RankDropdownMeta[settingId] = (RankAbilityCategory.가젯, category);
                }
            }

            int gearSettingId = nextSettingId++;
            dropdowns.Add(new DropdownSetting(
                id: gearSettingId,
                label: $"<color={RankAbilityCategoryExtensions.GetColor(RankAbilityCategory.기어)}>기어</color>",
                hintDescription: string.Join("\n", RankInfo.기어.Select(x => $"• {x.Key}: {x.Value.Item1}")),
                options: RankInfo.기어.Keys,
                header: RankHeader
            ));
            RankDropdownMeta[gearSettingId] = (RankAbilityCategory.기어, null);

            SettingBase.Register(dropdowns);
        }

        public static void OnSSInput(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            if (setting is not SSDropdownSetting dropdown)
                return;

            if (!RankDropdownMeta.TryGetValue(setting.SettingId, out var meta))
                return;

            Player player = Player.Get(sender);
            if (player is null)
                return;

            string selectedOption = dropdown.SyncSelectionText;
            if (string.IsNullOrWhiteSpace(selectedOption))
                return;

            RankCategory rankCategory = meta.RankCategory ?? default;
            RankAbilityType abilityType;

            if (meta.AbilityCategory == RankAbilityCategory.변칙성)
            {
                if (!meta.RankCategory.HasValue)
                    return;

                if (!RankInfo.변칙성.TryGetValue(meta.RankCategory.Value, out var anomalies) ||
                    !anomalies.TryGetValue(selectedOption, out var anomalyInfo))
                    return;

                abilityType = anomalyInfo.Item2;
            }
            else if (meta.AbilityCategory == RankAbilityCategory.가젯)
            {
                if (!meta.RankCategory.HasValue)
                    return;

                if (!RankInfo.가젯.TryGetValue(meta.RankCategory.Value, out var gadgets) ||
                    !gadgets.TryGetValue(selectedOption, out var gadgetInfo))
                    return;

                abilityType = gadgetInfo.Item2;
            }
            else
            {
                if (!RankInfo.기어.TryGetValue(selectedOption, out var gearInfo))
                    return;

                abilityType = gearInfo.Item2;
            }

            if (!RankInfo.PlayerRankSettingAbilities.ContainsKey(player))
                RankInfo.PlayerRankSettingAbilities[player] = new();

            if (!RankInfo.PlayerRankSettingAbilities[player].ContainsKey(rankCategory))
                RankInfo.PlayerRankSettingAbilities[player][rankCategory] = new();

            var list = RankInfo.PlayerRankSettingAbilities[player][rankCategory];

            if (!list.Contains(abilityType))
                list.Add(abilityType);
        }
    }
}
