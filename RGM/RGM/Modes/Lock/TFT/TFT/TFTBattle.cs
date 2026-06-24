using DAONTFT.Core.Classes;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using HintServiceMeow.Core.Extension;
using MEC;
using PlayerRoles;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using static DAONTFT.Core.Variables.Base;
using static RGM.Variables.Variable;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;
using Random = UnityEngine.Random;

namespace DAONTFT.Core.TFT;

public static class TFTBattle
{

    private static readonly Dictionary<Player, int> SelectionCursor = new();

    private static void ClearSelectionPresentation(Player player, bool playCloseAudio = false)
    {
        if (PlayerHints.TryGetValue(player, out var hints))
        {
            foreach (var hint in hints.Where(x => x.Id == "증강").ToList())
            {
                player.RemoveHint(hint);
                hints.Remove(hint);
            }
        }

        foreach (var clip in PlayersAudio[player].ClipsById.Values)
        {
            if (clip.Clip.Contains("증강"))
                clip.IsPaused = true;
        }

        player.DisableEffect(EffectType.Blinded);
        player.DisableEffect(EffectType.SoundtrackMute);

        if (playCloseAudio)
            PlayersAudio[player].TryPlay("증강 선택창 닫힘", 3);
    }

    public static Dictionary<string, string> RatingColor = new Dictionary<string, string>()
    {
        {"일반", "#A4A4A4"},
        {"희귀", "#2ECCFA"},
        {"영웅", "#FF00FF"},
        {"전설", "#ffd700"},
        {"신화", "#DF0101"},
        {"전용", "#F7819F"},
        {"시너지", "#DEEFED"}
    };
    public static Dictionary<string, string> SelectFormat = new Dictionary<string, string>()
    {
        {"일반", "<b><i><color=#404040>일</color><color=#474747>반</color> <color=#555555>(</color><color=#5C5C5C>C</color><color=#636363>o</color><color=#6B6B6B>m</color><color=#727272>m</color><color=#797979>o</color><color=#808080>n</color><color=#878787>)</color></i></b>"},
        {"희귀", "<b><i><color=#47DAFF>희</color><color=#47D4FC>귀</color> <color=#47C8F7>(</color><color=#47C2F5>R</color><color=#48BCF2>a</color><color=#48B6F0>r</color><color=#48B0ED>e</color><color=#48AAEB>)</color></i></b>"},
        {"영웅", "<b><i><color=#F185FF>영</color><color=#F27DFC>웅</color> <color=#F56EF6>(</color><color=#F767F3>E</color><color=#F85FF1>p</color><color=#FA58EE>i</color><color=#FB50EB>c</color><color=#FD49E8>)</color></i></b>"},
        {"전설", "<b><i><color=#FFF70A>전</color><color=#FFF40B>설</color> <color=#FFEE0E>(</color><color=#FFEC0F>L</color><color=#FFE911>e</color><color=#FFE612>g</color><color=#FFE314>e</color><color=#FFE115>n</color><color=#FFDE17>d</color><color=#FFDB18>)</color></i></b>"},
        {"신화", "<b><i><color=#F52500>신</color><color=#F12604>화</color> <color=#E9280D>(</color><color=#E52911>M</color><color=#E12A16>y</color><color=#DD2B1A>t</color><color=#D92C1F>h</color><color=#D52D23>i</color><color=#D12E28>c</color><color=#CD2F2C>)</color></i></b>"},
        {"알 수 없음", "<b><i><color=#000000>알</color> <color=#555555>수</color> <color=#AAAAAA>없</color><color=#D4D4D4>음</color></i></b>"}
    };

    public static string ColorFormat(string text)
    {
        return text.Replace("[시너지]", $"<color={RatingColor["시너지"]}>[시너지]</color>")
                    .Replace("[전용]", $"<color={RatingColor["전용"]}>[전용]</color>")
                    .Replace("[신화]", $"<color={RatingColor["신화"]}>[신화]</color>")
                    .Replace("[전설]", $"<color={RatingColor["전설"]}>[전설]</color>")
                    .Replace("[영웅]", $"<color={RatingColor["영웅"]}>[영웅]</color>")
                    .Replace("[희귀]", $"<color={RatingColor["희귀"]}>[희귀]</color>")
                    .Replace("[일반]", $"<color={RatingColor["일반"]}>[일반]</color>");
    }

    public static IEnumerator<float> RestoreAbilities(List<Player> players)
    {
        foreach (var player in players)
        {
            List<TFTAbilityType> _abilities = PlayerTFTAbilities[player].Select(x => x.Data.TFTAbilityType).ToList();

            Reset(player);

            yield return Timing.WaitForOneFrame;

            foreach (var TFTAbility in _abilities)
                player.AddTFTAbility(TFTAbility);

            yield return Timing.WaitForOneFrame;

            player.AddBroadcast(10, $"<size=25><b>모든 능력을 제거한 후, 수복하였습니다.</b></size>");
        }
    }

    public static TFTAbilityLevel GetRandomAbilityLevel()
    {
        int random = Random.Range(1, 101);

        if (random <= 15)
            return TFTAbilityLevel.Keter;

        if (random <= 50)
            return TFTAbilityLevel.Euclid;

        return TFTAbilityLevel.Safe;
    }

    public static bool StartUpgrade(List<Player> players = null, TFTAbilityLevel? levelOverride = null)
    {
        TFTAbilityLevel level = levelOverride ?? GetRandomAbilityLevel();
        bool started = false;

        foreach (var player in players == null ? PlayerManager.List.Where(x => x.IsAlive && x.GetAbilities().Count() < (Encounter == RoleTypeId.Scp0492 ? 4 : 3)) : players)
        {
            TFTAbilityLevel playerLevel = level;

            if (Encounter == RoleTypeId.NtfCaptain)
                playerLevel = TFTAbilityLevel.Keter;

            if (Encounter == RoleTypeId.NtfSergeant)
                playerLevel = TFTAbilityLevel.Euclid;

            if (Encounter == RoleTypeId.NtfSpecialist && player.GetAbilities().Count() == 0)
                playerLevel = TFTAbilityLevel.Keter;

            if (Encounter == RoleTypeId.NtfPrivate && player.GetAbilities().Count() == 2)
                playerLevel = TFTAbilityLevel.Keter;

            Dictionary<TFTAbilityData, int> TFTAbilityDatas = player.GetValidAbilities(playerLevel);
            Dictionary<TFTAbilityType, int> abilities = new();

            foreach (var TFTAbility in TFTAbilityDatas)
            {
                abilities.Add(TFTAbility.Key.TFTAbilityType, TFTAbility.Value);
            }

            if (abilities.Count == 0)
                continue;

            player.StartSelect(abilities, Math.Min(2, abilities.Count));
            started = true;
        }

        return started;
    }

    // 플레이어에게 특정 능력을 부여
    public static void AddTFTAbility(this Player player, TFTAbilityType type)
    {
        Log.Info("AddTFTAbility called with " + player.DisplayNickname + " and " + type);

        if (!TFTAbilities.ContainsKey(type))
        {
            Log.Error($"TFTAbility {type} not found.");

            return;
        }

        if (!PlayerTFTAbilities.ContainsKey(player))
        {
            Log.Info("No key");
            PlayerTFTAbilities.Add(player, []);
        }

        var TFTAbilityData = TFTAbilities[type];
        TFTAbility TFTAbility;

        try
        {
             TFTAbility = Activator.CreateInstance(TFTAbilities[type].Type) as TFTAbility;
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred while trying to create an instance of {TFTAbilityData.Name}: {e}");
            return;
        }

        if (TFTAbility == null)
        {
            Log.Error($"An error occurred while trying to create an instance of {TFTAbilityData.Name}. The instance is null.");
            return;
        }

        TFTAbility.Data = TFTAbilityData;
        TFTAbility.Owner = player;
        TFTAbility.OnEnabled();

        PlayerTFTAbilities[player].Add(TFTAbility);

        string styleName = ColorFormat(TFTAbilityData.GetFormattedName());

        string Message = $"<size=20>{styleName}</size>\n<size=15>{TFTAbilityData.Description}</size>";
        //player.AddBroadcast(10, Message);
        player.SendConsoleMessage($"\n{Message}", "white");
    }

    // 플레이어로부터 특정 능력 제거
    public static void RemoveTFTAbility(this Player player, TFTAbilityType type)
    {
        if (!PlayerTFTAbilities.TryGetValue(player, out var playerTFTAbility))
            return;

        var TFTAbility = playerTFTAbility.FirstOrDefault(x => x.Data.TFTAbilityType == type);

        if (TFTAbility == null)
            return;

        TFTAbility.OnDisabled();
        PlayerTFTAbilities[player].Remove(TFTAbility);
    }

    public static void RemoveTFTAbility(this Player player, TFTAbility TFTAbility)
    {
        if (TFTAbility == null)
            return;

        if (!PlayerTFTAbilities.TryGetValue(player, out var playerTFTAbility))
            return;

        if (!playerTFTAbility.Contains(TFTAbility))
            return;

        TFTAbility.OnDisabled();
        PlayerTFTAbilities[player].Remove(TFTAbility);
    }

    // 플레이어로부터 모든 능력 제거
    public static void RemoveAllAbilities(this Player player)
    {
        if (!PlayerTFTAbilities.TryGetValue(player, out var playerTFTAbility))
            return;

        foreach (var TFTAbility in playerTFTAbility)
            TFTAbility.OnDisabled();

        PlayerTFTAbilities.Remove(player);
    }

    // 플레이어의 모든 능력 가져오기
    public static List<TFTAbility> GetAbilities(this Player player)
    {
        return PlayerTFTAbilities.TryGetValue(player, out var playerTFTAbility) ? playerTFTAbility : new List<TFTAbility>();
    }

    public static TFTAbilityType FindTFTAbility(string name)
    {
        return TFTAbilities.FirstOrDefault(x => x.Value.Name == name).Key;
    }

    // 플레이어의 특정 능력 가져오기
    public static TFTAbility GetTFTAbility(this Player player, TFTAbilityType type)
    {
        return GetAbilities(player).FirstOrDefault(x => x.Data.TFTAbilityType == type);
    }

    // 플레이어가 특정 능력을 가지고 있는지 확인
    public static bool HasTFTAbility(this Player player, TFTAbilityType type)
    {
        return GetTFTAbility(player, type) != null;
    }

    public static List<TFTAbilityType> GetRandomAbilities(TFTAbilityLevel level, int count)
    {
        var abilities = TFTAbilities.Where(x => x.Value.Level == level).ToList();

        abilities.ShuffleList();

        var result = new List<TFTAbilityType>();
        for (int i = 0; i < count; i++)
        {
            var picked = abilities.GetRandomValue().Key;
            result.Add(picked);
        }
        return result;
    }

    public static void StartSelect(this Player player, Dictionary<TFTAbilityType, int> abilities, int count = 2)
    {
        if (!Selections.ContainsKey(player))
            Selections.Add(player, new Dictionary<TFTAbilityType, int>());

        var ignoredIndexes = new List<int>();

        if (abilities.Count == 0)
        {
            IsSelecting[player] = false;
            return;
        }

        IsSelecting[player] = true;

        Dictionary<TFTAbilityType, int> queue = new();
        int selectionCount = Math.Min(count, abilities.Count);
        List<TFTAbilityType> candidates = abilities.Keys.ToList();
        candidates.ShuffleList();

        foreach (var TFTAbility in candidates.Take(selectionCount))
        {
            queue.Add(TFTAbility, Encounter == RoleTypeId.Tutorial ? 3 : 1);
        }

        Selections[player] = queue;
        SelectionCursor[player] = 0;

        // 다음 타자, 코루틴!!!
        Timing.RunCoroutine(SelectionCoroutine(player));
    }

    private static IEnumerator<float> SelectionCoroutine(this Player player)
    {
        TFTAbilityLevel level = TFTAbilityLevel.Safe;

        PlayersAudio[player].TryPlay("증강 선택창", 3);

        player.EnableEffect(EffectType.Blinded, 30);
        player.EnableEffect(EffectType.SoundtrackMute);

        List<TFTAbilityType> getAbilities() 
        {
            return Selections[player].Keys.ToList();
        };

        var text = string.Join("\n", getAbilities().Select((x, i) => $"[{i + 1}] {x.GetData().GetFormattedName()}\n<size=20>{TFTAbilities[x].Description}</size>\n"));

        List<Upgrade> upgrades = new();

        void TFTAbilityAdd(TFTAbilityType TFTAbility)
        {
            upgrades.Add(new Upgrade
            {
                X = -400 + getAbilities().IndexOf(TFTAbility) * 800, //-1200 + getAbilities().IndexOf(TFTAbility) * 800,
                Emoji = TFTAbility.GetData().Emoji,
                Title = TFTAbility.GetData().Name,
                Description = TFTAbility.GetData().Description,
                Level = TFTAbility.GetData().Level,
                Type = TFTAbility.GetData().TFTAbilityType
            });
        }

        foreach (var TFTAbility in getAbilities())
        {
            TFTAbilityAdd(TFTAbility);

            level = TFTAbility.GetData().Level;
        }

        PlayersAudio[player].TryPlay($"{level.ToString()} 증강", 0.7f);

        void removeHints()
        {
            foreach (var hint in PlayerHints[player].Where(x => x.Id == "증강"))
            {
                player.RemoveHint(hint);
            }
        }

        IEnumerator<float> hintDisplay()
        {
            void resetupgradeDisplay()
            {
                upgrades.Clear();

                foreach (var TFTAbility in getAbilities())
                {
                    TFTAbilityAdd(TFTAbility);
                }

                removeHints();

                var abilities = getAbilities();
                TFTAbilityType? selected = null;

                if (abilities.Count > 0)
                {
                    if (!SelectionCursor.ContainsKey(player))
                        SelectionCursor[player] = 0;

                    int cursor = Math.Max(0, Math.Min(SelectionCursor[player], abilities.Count - 1));
                    SelectionCursor[player] = cursor;
                    selected = abilities[cursor];
                }

                List<Hint> hints = TFTManager.GetUpgradeDisplay(player, upgrades, 0, selected);

                foreach (var hint in hints)
                {
                    PlayerHints[player].Add(hint);
                    player.AddCustomHint(hint);
                }
            }

            while (true)
            {
                resetupgradeDisplay();

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        CoroutineHandle _hintDisplay = Timing.RunCoroutine(hintDisplay());

        for (var i = 0; i < 300; i++)
        {
            if (player.IsDead || !Selections.ContainsKey(player))
            {
                if (Selections.ContainsKey(player))
                    Selections.Remove(player);

                SelectionCursor.Remove(player);
                IsSelecting[player] = false;

                Timing.KillCoroutines(_hintDisplay);
                ClearSelectionPresentation(player);

                yield break;
            }

            yield return Timing.WaitForSeconds(0.1f);
        }

        IsSelecting[player] = false;
        Timing.KillCoroutines(_hintDisplay);
        removeHints();
        player.DisableEffect(EffectType.Blinded);
        player.DisableEffect(EffectType.SoundtrackMute);
        PlayersAudio[player].TryPlay($"증강 선택창 닫힘", 3);

        if (!Selections.ContainsKey(player))
            yield break;

        var random = Random.Range(0, getAbilities().Count);

        player.AddTFTAbility(getAbilities().ElementAt(random));

        Selections.Remove(player);
        SelectionCursor.Remove(player);
    }

    public static bool Select(this Player player, int index, out string response)
    {
        if (!Selections.ContainsKey(player))
        {
            response = "선택할 수 있는 능력이 없습니다.";
            return false;
        }

        Log.Info("Select called with " + player.Nickname + " and " + index);

        TFTAbilityType TFTAbility;

        Log.Info("All abilities are not the same");

        TFTAbility = Selections[player].Keys.ElementAt(index - 1);

        player.AddTFTAbility(TFTAbility);

        Selections.Remove(player);
        SelectionCursor.Remove(player);

        ClearSelectionPresentation(player, true);

        response = $"{index}번 능력 선택 완료!";
        return true;
    }

    public static bool Reroll(this Player player, int index, out string response)
    {
        if (!Selections.ContainsKey(player))
        {
            response = "선택할 수 있는 능력이 없습니다.";
            return false;
        }

        Log.Info("Select called with " + player.Nickname + " and " + index);

        TFTAbilityType TFTAbility;

        Log.Info("All abilities are not the same");

        TFTAbility = Selections[player].Keys.ElementAt(index - 1);

        if (Selections[player][TFTAbility] > 0)
        {
            Selections[player][TFTAbility]--;

            int chance = Selections[player][TFTAbility];

            Dictionary<TFTAbilityData, int> TFTAbilityDatas = player.GetValidAbilities(TFTAbility.GetData().Level);

            Selections[player].Remove(TFTAbility);
            Selections[player].Add(TFTAbilityDatas.GetRandomValue().Key.TFTAbilityType, chance);

            PlayersAudio[player].TryPlay($"증강 선택창 리롤", 2.5f);
            response = $"{index}번 능력 리롤 완료!";
            return true;
        }
        else
        {
            response = "이 선택지에서 더 이상 리롤할 수 없습니다.";
            return false;
        }
    }

    public static void MoveSelectionCursor(this Player player, int delta)
    {
        if (!Selections.TryGetValue(player, out var selections) || selections.Count == 0)
            return;

        if (!SelectionCursor.ContainsKey(player))
            SelectionCursor[player] = 0;

        int cursor = SelectionCursor[player];
        cursor = (cursor + delta) % selections.Count;

        if (cursor < 0)
            cursor += selections.Count;

        SelectionCursor[player] = cursor;

        PlayersAudio[player].TryPlay("Select");
    }

    public static bool ConfirmSelectionByCursor(this Player player, out string response)
    {
        if (!Selections.TryGetValue(player, out var selections) || selections.Count == 0)
        {
            response = "선택할 수 있는 능력이 없습니다.";
            return false;
        }

        if (!SelectionCursor.ContainsKey(player))
            SelectionCursor[player] = 0;

        int cursor = Math.Max(0, Math.Min(SelectionCursor[player], selections.Count - 1));
        return player.Select(cursor + 1, out response);
    }

    public static void Reset(this Player player)
    {
        player.RemoveAllAbilities();

        if (Selections.ContainsKey(player))
            Selections.Remove(player);

        SelectionCursor.Remove(player);
        IsSelecting[player] = false;
        IsLifeUsed[player] = false;

        ClearSelectionPresentation(player);
    }
}