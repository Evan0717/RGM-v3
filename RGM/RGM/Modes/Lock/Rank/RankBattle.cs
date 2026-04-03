using DAONTFT.Core.TFT;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using MEC;
using RGM.API.Features;
using System;
using System.Collections.Generic;

namespace RGM.Modes
{
    public static class RankBattle
    {
        public static void AddRankAbility(Player player, RankAbilityType type)
        {
            Log.Info("AddRankAbility called with " + player.DisplayNickname + " and " + type);

            if (!RankInfo.RankAbilities.ContainsKey(type))
            {
                Log.Error($"RankAbility {type} not found.");

                return;
            }

            var RankAbilityData = RankInfo.RankAbilities[type];
            RankAbility RankAbility;

            try
            {
                RankAbility = Activator.CreateInstance(RankInfo.RankAbilities[type].Type) as RankAbility;
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred while trying to create an instance of {RankAbilityData.Name}: {e}");
                return;
            }

            if (RankAbility == null)
            {
                Log.Error($"An error occurred while trying to create an instance of {RankAbilityData.Name}. The instance is null.");
                return;
            }

            RankInfo.PlayerRankAbilities[player].Add(RankAbility);

            RankAbility.Data = RankAbilityData;
            RankAbility.Owner = player;
            RankAbility.OnEnabled();
        }

        public static void RemoveAllRankAbilities(this Player player)
        {
            if (!RankInfo.PlayerRankAbilities.TryGetValue(player, out var playerRankAbility))
                return;

            foreach (var rankAbility in playerRankAbility)
                rankAbility.OnDisabled();

            RankInfo.PlayerRankSettingAbilities.Remove(player);
        }

        public static void Reset(this Player player)
        {
            player.RemoveAllRankAbilities();
        }

        public static IEnumerator<float> UpgradeDisplay(Player Owner)
        {
            string hintDescription = $"[ESC] -> [Settings] -> [Server-specific]ㅣ<color={RankAbilityCategory.변칙성.GetColor()}>변칙성</color>, <color={RankAbilityCategory.가젯.GetColor()}>가젯</color>, <color={RankAbilityCategory.기어.GetColor()}>기어</color>를 미리 설정해두세요.";

            HintServiceMeow.Core.Models.Hints.Hint hint = new HintServiceMeow.Core.Models.Hints.Hint
            {
                Text = $"",
            };

            while (true)
            {
                if (Owner.IsAlive)
                {
                    List<string> queue = new();

                    if (RankInfo.PlayerRankAbilities.TryGetValue(Owner, out var abilities))
                    {
                        foreach (var ability in abilities)
                        {
                            queue.Add($"{ability.Data.GetFormattedName()}ㅣ{ability.Data.Description}");
                        }
                    }
                    else
                    {
                        queue.Add(hintDescription);
                    }

                    hint = new HintServiceMeow.Core.Models.Hints.Hint
                    {
                        Text = $"<size=15>{string.Join("\n", queue)}</size>",
                        Id = "능력 리스트",
                        XCoordinate = 300,
                        YCoordinate = 100,
                        Alignment = HintAlignment.Left
                    };

                    Owner.AddCustomHint(hint);

                    yield return Timing.WaitForSeconds(1);

                    Owner.RemoveHint(hint);
                }
                else if (Owner.Role is SpectatorRole spectator && spectator.SpectatedPlayer != null)
                {
                    List<string> queue = new();

                    if (RankInfo.PlayerRankAbilities.TryGetValue(spectator.SpectatedPlayer, out var abilities))
                    {
                        foreach (var ability in abilities)
                        {
                            queue.Add($"{ability.Data.GetFormattedName()}ㅣ{ability.Data.Description}");
                        }
                    }
                    else
                    {
                        queue.Add(hintDescription);
                    }

                    hint = new HintServiceMeow.Core.Models.Hints.Hint
                    {
                        Text = $"<size=15>{string.Join("\n", queue)}</size>",
                        Id = "능력 리스트",
                        XCoordinate = 300,
                        YCoordinate = 100,
                        Alignment = HintAlignment.Left
                    };

                    Owner.AddCustomHint(hint);

                    yield return Timing.WaitForSeconds(1);

                    Owner.RemoveHint(hint);
                }
                else
                {
                    yield return Timing.WaitForSeconds(1);
                }
            }
        }
    }
}
