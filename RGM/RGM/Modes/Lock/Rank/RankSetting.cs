using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace RGM.Modes
{
    public static class RankSetting
    {
        private const int RankSettingIdStart = 2044;
        private static readonly Dictionary<int, (RankAbilityCategory AbilityCategory, RankCategory? RankCategory)> RankDropdownMeta = new();

        public static HeaderSetting RankHeader { get; private set; } = new HeaderSetting(190023, "경쟁전");

        public static KeybindSetting RankInfoKey { get; private set; }
        public static KeybindSetting GadgetKey { get; private set; }

        private static string GetFormattedOption(string name, RankAbilityType type, bool showCooldown = false)
        {
            var data = type.GetData();
            if (data is null)
                return name;

            string result = $"{data.Emoji} {name}";

            if (showCooldown && data.Type != null && data.Type.GetCustomAttribute<RankGadgetAttribute>() is RankGadgetAttribute gadgetAttribute)
                result += $" ({gadgetAttribute.Cooldown}초)";

            return result;
        }

        public static void Init()
        {
            List<SettingBase> list = new();

            RankInfoKey = new KeybindSetting(
               id: 289289922,
               label: "<b>경쟁전 정보 키</b>",
               suggested: KeyCode.F1,
               preventInteractionOnGUI: true,
               hintDescription: "자신의 정보를 볼 때 누르는 키입니다.",
               header: RankHeader
               );

            GadgetKey = new KeybindSetting(
                id: 289289923,
                label: "<b>경쟁전 가젯 키</b>",
                suggested: KeyCode.Z,
                preventInteractionOnGUI: true,
                hintDescription: "가젯을 사용할 때 누르는 키입니다.",
                header: RankHeader
                );

            SettingBase[] settings = new SettingBase[]
            {
                RankInfoKey,
                GadgetKey
            };

            foreach (var setting in settings)
                list.Add(setting);

            // ---------------------------------------------------------------------------------------------------------------------------------------------

            RankDropdownMeta.Clear();
            int nextSettingId = RankSettingIdStart;

            var categories = RankInfo.변칙성.Keys.Union(RankInfo.가젯.Keys).Distinct();

            foreach (var category in categories)
            {
                string roleName = category.ToString().Contains("SCP") ? category.ToString().Replace("_", "-") : category.ToString().Replace("_", " ");

                if (RankInfo.변칙성.TryGetValue(category, out var anomalyDict))
                {
                    int settingId = nextSettingId++;
                    list.Add(new DropdownSetting(
                        id: settingId,
                        label: $"<color={RankAbilityCategoryExtensions.GetColor(RankAbilityCategory.변칙성)}>변칙성</color> <color={category.GetRoleCategory().GetColor().ToHex()}>{roleName}</color>",
                        hintDescription: string.Join("\n", anomalyDict.Select(x => $"• {GetFormattedOption(x.Key, x.Value.Item2)}: {x.Value.Item1}")),
                        options: anomalyDict.Select(x => GetFormattedOption(x.Key, x.Value.Item2)),
                        header: RankHeader
                    ));
                    RankDropdownMeta[settingId] = (RankAbilityCategory.변칙성, category);
                }

                if (RankInfo.가젯.TryGetValue(category, out var gadgetDict))
                {
                    int settingId = nextSettingId++;
                    list.Add(new DropdownSetting(
                        id: settingId,
                        label: $"<color={RankAbilityCategoryExtensions.GetColor(RankAbilityCategory.가젯)}>가젯</color> <color={category.GetRoleCategory().GetColor().ToHex()}>{roleName}</color>",
                        hintDescription: string.Join("\n", gadgetDict.Select(x => $"• {GetFormattedOption(x.Key, x.Value.Item2, true)}: {x.Value.Item1}")),
                        options: gadgetDict.Select(x => GetFormattedOption(x.Key, x.Value.Item2, true)),
                        header: RankHeader
                    ));
                    RankDropdownMeta[settingId] = (RankAbilityCategory.가젯, category);
                }
            }

            int gearSettingId = nextSettingId++;
            list.Add(new DropdownSetting(
                id: gearSettingId,
                label: $"<color={RankAbilityCategoryExtensions.GetColor(RankAbilityCategory.기어)}>기어</color>",
                hintDescription: string.Join("\n", RankInfo.기어.Select(x => $"• {GetFormattedOption(x.Key, x.Value.Item2)}: {x.Value.Item1}")),
                options: RankInfo.기어.Select(x => GetFormattedOption(x.Key, x.Value.Item2)),
                header: RankHeader
            ));
            RankDropdownMeta[gearSettingId] = (RankAbilityCategory.기어, null);

            SettingBase.Register(list);
        }

        public static void OnSSInput(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            Player player = Player.Get(sender);

            if (setting is SSKeybindSetting keybind) 
            {
                if (keybind.SyncIsPressed)
                {
                    if (keybind.SettingId == 289289922)
                    {
                        if (!RankInfo.PlayerShowRanks.ContainsKey(player))
                            RankInfo.PlayerShowRanks.Add(player, false);

                        if (RankInfo.PlayerShowRanks[player])
                            RankInfo.PlayerShowRanks[player] = false;

                        else
                            RankInfo.PlayerShowRanks[player] = true;

                    }
                }
            }

            if (setting is SSDropdownSetting dropdown)
            {
                if (!RankDropdownMeta.TryGetValue(setting.SettingId, out var meta))
                    return;

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

                    if (!RankInfo.변칙성.TryGetValue(meta.RankCategory.Value, out var anomalies))
                        return;

                    var match = anomalies.FirstOrDefault(x => GetFormattedOption(x.Key, x.Value.Item2) == selectedOption);
                    if (match.Key == null)
                        return;

                    abilityType = match.Value.Item2;
                }
                else if (meta.AbilityCategory == RankAbilityCategory.가젯)
                {
                    if (!meta.RankCategory.HasValue)
                        return;

                    if (!RankInfo.가젯.TryGetValue(meta.RankCategory.Value, out var gadgets))
                        return;

                    var match = gadgets.FirstOrDefault(x => GetFormattedOption(x.Key, x.Value.Item2, true) == selectedOption);
                    if (match.Key == null)
                        return;

                    abilityType = match.Value.Item2;
                }
                else
                {
                    var match = RankInfo.기어.FirstOrDefault(x => GetFormattedOption(x.Key, x.Value.Item2) == selectedOption);
                    if (match.Key == null)
                        return;

                    abilityType = match.Value.Item2;
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
}
