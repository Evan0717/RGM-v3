using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using PlayerRoles;
using RemoteAdmin;
using RGM.Modes.Commands;
using Random = UnityEngine.Random;

namespace RGM.Modes;

public class ABattle
{
    public static ABattle Instance { get; set; }

    public Dictionary<Player, List<WorkstationController>> PlayerWorkstations { get; set; }
    public Dictionary<AbilityType, AbilityData> Abilities { get; set; }
    public Dictionary<AbilityType, List<AbilityType>> SynergyAbilities;
    public Dictionary<Player, List<Ability>> PlayerAbilities;
    public Dictionary<Player, List<AbilityType>> Selections;
    public Dictionary<Player, bool> IsSelecting;
    public Dictionary<Player, bool> IsLifeUsed;

    public bool IsFeverModeEnabled;

    private ABattleEventHandler _eventHandler;

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

    // 플러그인에 있는 모든 능력 검색
    public void OnEnabled()
    {
        Instance = this;

        _eventHandler = new ABattleEventHandler(this);
        _eventHandler.RegisterEvents();

        PlayerAbilities = new Dictionary<Player, List<Ability>>();
        SynergyAbilities = new Dictionary<AbilityType, List<AbilityType>>();
        Selections = new Dictionary<Player, List<AbilityType>>();
        IsSelecting = new Dictionary<Player, bool>();
        IsLifeUsed = new Dictionary<Player, bool>();

        Abilities = new Dictionary<AbilityType, AbilityData>();
        PlayerWorkstations = new Dictionary<Player, List<WorkstationController>>();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var abilityAttribute = type.GetCustomAttribute<AbilityAttribute>();

            if (abilityAttribute == null)
                continue;

            if (!typeof(Ability).IsAssignableFrom(type))
                continue;

            Abilities.Add(abilityAttribute.Type, new AbilityData
            {
                Type = type,
                Name = abilityAttribute.Name,
                Description = abilityAttribute.Description,
                Category = abilityAttribute.Category,
                AbilityType = abilityAttribute.Type,
                Keep = abilityAttribute.Keep
            });

            var requiresAbilityAttribute = type.GetCustomAttribute<RequiresAbilityAttribute>();

            if (requiresAbilityAttribute != null && requiresAbilityAttribute.Abilities.Length > 0)
            {
                SynergyAbilities.Add(abilityAttribute.Type, requiresAbilityAttribute.Abilities.ToList());

                Abilities[abilityAttribute.Type].Category = AbilityCategory.Synergy;
                Abilities[abilityAttribute.Type].Requires = requiresAbilityAttribute.Abilities.ToList();
            }
        }

        QueryProcessor.DotCommandHandler.RegisterCommand(new SelectFirst());
        QueryProcessor.DotCommandHandler.RegisterCommand(new SelectSecond());
        QueryProcessor.DotCommandHandler.RegisterCommand(new SelectThird());

        CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(new Commands.AddAbility());

        Timing.RunCoroutine(OnModeStarted());
        Timing.RunCoroutine(HintCoroutine());
    }

    private IEnumerator<float> OnModeStarted()
    {
        if (Random.Range(1, 6) == 1)
            IsFeverModeEnabled = true;

        if (IsFeverModeEnabled)
            Server.ExecuteCommand("/mp load ABattle");

        foreach (var player in Player.List)
        {
            PlayerWorkstations.Add(player, new List<WorkstationController>());
            PlayerAbilities.Add(player, new List<Ability>());
            IsSelecting.Add(player, false);
            IsLifeUsed.Add(player, false);
        }

        yield break;
    }

    private IEnumerator<float> HintCoroutine()
    {
        while (true)
        {
            foreach (var player in Player.List.Where(x => !x.IsNPC))
            {
                var CurrentHint = player.CurrentHint;
                var isStatusHint = CurrentHint != null && (CurrentHint.Content.Contains("워크스테이션") || CurrentHint.Content.Contains("보유 업그레이드"));

                if (CurrentHint == null || isStatusHint)
                {
                    if (player.IsAlive)
                        player.ShowHint(FormatHint(player), 1.2f);
                }
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }

    private string FormatHint(Player player)
    {
        if (!PlayerAbilities.TryGetValue(player, out var ability))
        {
            return player.Role.Type == RoleTypeId.Scp079
                ? "<align=left><b><size=22>레벨이 오를 때마다 능력을 획득할 수 있습니다.</size></b></align>"
                : "<align=left><b><size=22>워크스테이션 위에서 점프하면 능력을 획득할 수 있습니다.</size></b></align>";
        }

        if (!ability.Any())
        {
            return player.Role.Type == RoleTypeId.Scp079
                ? "<align=left><b><size=22>레벨이 오를 때마다 능력을 획득할 수 있습니다.</size></b></align>"
                : "<align=left><b><size=22>워크스테이션 위에서 점프하면 능력을 획득할 수 있습니다.</size></b></align>";
        }

        var abilitiesText = string.Join(", ",
            PlayerAbilities[player]
                .GroupBy(x => x.Data.AbilityType)
                .Select(g => g.Count() > 1
                    ? $"{g.First().Data.GetFormattedName()} x{g.Count()}"
                    : g.First().Data.GetFormattedName())
                .ToList());

        return $"<align=left><b><size=25>보유 업그레이드</size></b>\n<size=20>{abilitiesText}</size></align>";
    }

    // 플레이어에게 특정 능력을 부여
    public void AddAbility(Player player, AbilityType type)
    {
        Log.Info("AddAbility called with " + player.Nickname + " and " + type);

        if (!Abilities.ContainsKey(type))
        {
            Log.Error($"Ability {type} not found.");

            return;
        }

        if (!PlayerAbilities.ContainsKey(player))
        {
            Log.Info("No key");
            PlayerAbilities.Add(player, []);
        }

        var abilityData = Abilities[type];
        Ability ability;

        try
        {
             ability = Activator.CreateInstance(Abilities[type].Type) as Ability;
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred while trying to create an instance of {abilityData.Name}: {e}");
            return;
        }

        if (ability == null)
        {
            Log.Error($"An error occurred while trying to create an instance of {abilityData.Name}. The instance is null.");
            return;
        }

        ability.Data = abilityData;
        ability.Owner = player;
        ability.OnEnabled();

        PlayerAbilities[player].Add(ability);

        EnableSynergyAbility(PlayerAbilities[player]);
    }

    // 플레이어에게 시너지 능력 부여
    private void EnableSynergyAbility(List<Ability> abilities)
    {
        foreach (var synergy in SynergyAbilities.Where(synergy =>
                     synergy.Value.All(req => abilities.Any(a => a.Data.AbilityType == req))))
        {
            if (!Abilities.TryGetValue(synergy.Key, out var synergyAbilityType))
                continue;

            if (abilities.Any(a => a.Data.AbilityType == synergy.Key))
                continue;

            Ability synergyAbility;
            try
            {
                synergyAbility = Activator.CreateInstance(synergyAbilityType.Type) as Ability;
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred while trying to create an instance of {synergyAbilityType.Name}: {e}");
                continue;
            }

            if (synergyAbility == null)
                continue;

            synergyAbility.OnEnabled();
            abilities.Add(synergyAbility);
        }
    }

    // 플레이어로부터 특정 능력 제거
    public void RemoveAbility(Player player, AbilityType type)
    {
        if (!PlayerAbilities.TryGetValue(player, out var playerAbility))
            return;

        var ability = playerAbility.FirstOrDefault(x => x.Data.AbilityType == type);

        if (ability == null)
            return;

        ability.OnDisabled();
        PlayerAbilities[player].Remove(ability);

        RemoveSynergyAbility(PlayerAbilities[player]);
    }

    public void RemoveAbility(Player player, Ability ability)
    {
        if (ability == null)
            return;

        if (!PlayerAbilities.TryGetValue(player, out var playerAbility))
            return;

        if (!playerAbility.Contains(ability))
            return;

        ability.OnDisabled();
        PlayerAbilities[player].Remove(ability);

        RemoveSynergyAbility(PlayerAbilities[player]);
    }

    // 플레이어로부터 시너지 능력 확인 후 제거
    private void RemoveSynergyAbility(List<Ability> abilities)
    {
        foreach (var synergy in SynergyAbilities)
        {
            if (!synergy.Value.All(req => abilities.Any(a => a.Data.AbilityType == req)) &&
                abilities.Any(a => a.Data.AbilityType == synergy.Key))
            {
                var ability = abilities.First(a => a.Data.AbilityType == synergy.Key);
                ability.OnDisabled();
                abilities.Remove(ability);
            }
        }
    }

    // 플레이어로부터 모든 능력 제거
    public void RemoveAllAbilities(Player player)
    {
        if (!PlayerAbilities.TryGetValue(player, out var playerAbility))
            return;

        foreach (var ability in playerAbility)
            ability.OnDisabled();

        PlayerAbilities.Remove(player);
    }

    // 플레이어의 모든 능력 가져오기
    public List<Ability> GetAbilities(Player player)
    {
        return PlayerAbilities.TryGetValue(player, out var playerAbility) ? playerAbility : new List<Ability>();
    }

    public AbilityType FindAbility(string name)
    {
        return Abilities.FirstOrDefault(x => x.Value.Name == name).Key;
    }

    // 플레이어의 특정 능력 가져오기
    public Ability GetAbility(Player player, AbilityType type)
    {
        return GetAbilities(player).FirstOrDefault(x => x.Data.AbilityType == type);
    }

    // 플레이어가 특정 능력을 가지고 있는지 확인
    public bool HasAbility(Player player, AbilityType type)
    {
        return GetAbility(player, type) != null;
    }

    public List<AbilityType> GetRandomAbilities(AbilityCategory category, int count)
    {
        var abilities = Abilities.Where(x => x.Value.Category == category).ToList();

        if (category == AbilityCategory.None)
            abilities = Abilities.ToList();

        abilities.ShuffleList();

        return abilities.Take(count).Select(x => x.Key).ToList();
    }

    public void StartSelect(Player player)
    {
        if (!Selections.ContainsKey(player))
            Selections.Add(player, new List<AbilityType>());

        IsSelecting[player] = true;

        var category = GetCategory(player);

        if (category == AbilityCategory.None)
            return;

        var abilities = GetRandomAbilities(category, 3);
        var ignoredIndexes = new List<int>();

        if (abilities.Count == 0)
            return;

        if (player.HasAbility(AbilityType.RARE_TRANSITION))
        {
            player.RemoveAbility(AbilityType.RARE_TRANSITION);

            var transition = Random.Range(1, 5) == 1;

            if (transition)
            {
                int index;

                do
                {
                    index = Random.Range(0, 3);
                } while (ignoredIndexes.Contains(index));

                ignoredIndexes.Add(index);

                var ability = GetRandomAbilities(AbilityCategory.Epic, 1).First();

                abilities[index] = ability;
            }
        }

        if (player.HasAbility(AbilityType.EPIC_TRANSITION))
        {
            player.RemoveAbility(AbilityType.EPIC_TRANSITION);

            var transition = Random.Range(1, 5) == 1;

            if (transition)
            {
                int index;

                do
                {
                    index = Random.Range(0, 3);
                } while (ignoredIndexes.Contains(index));

                ignoredIndexes.Add(index);

                var ability = GetRandomAbilities(AbilityCategory.Legend, 1).First();

                abilities[index] = ability;
            }
        }

        if (player.HasAbility(AbilityType.LEGEND_TRANSITION))
        {
            player.RemoveAbility(AbilityType.LEGEND_TRANSITION);

            var transition = Random.Range(1, 5) == 1;

            if (transition)
            {
                int index;

                do
                {
                    index = Random.Range(0, 3);
                } while (ignoredIndexes.Contains(index));

                ignoredIndexes.Add(index);

                var ability = GetRandomAbilities(AbilityCategory.Mythic, 1).First();

                abilities[index] = ability;
            }
        }

        Selections[player] = abilities;

        // 다음 타자, 코루틴!!!
        Timing.RunCoroutine(SelectionCoroutine(player));
    }

    private IEnumerator<float> SelectionCoroutine(Player player)
    {
        var abilities = Selections[player];
        var text = string.Join("\n", abilities.Select((x, i) => $"[{i + 1}] {x.GetTranslation()}\n<size=20>{Abilities[x].Description}</size>\n"));

        string CheckAbilityGrade()
        {
            if (text.Contains("일반")) return "일반";
            else if (text.Contains("희귀")) return "희귀";
            else if (text.Contains("영웅")) return "영웅";
            else if (text.Contains("전설")) return "전설";
            else if (text.Contains("신화")) return "신화";

            else return "알 수 없음";
        }

        for (var i = 0; i < 20; i++)
        {
            if (player.IsDead || !Selections.ContainsKey(player))
            {
                IsSelecting[player] = false;

                yield break;
            }

            player.ShowHint(
            $"<align=left><size=40><b>능력 선택창ㅣ{SelectFormat[CheckAbilityGrade()]}</b></size>\n\n<size=30>{text}</size>\n\n<size=25><b>{20 - i}초 안에 [.(번호)] 명령어로 원하는 능력을 선택하세요. (ex .1)</b></size></align>\n\n\n\n\n",
            1.2f);

            yield return Timing.WaitForSeconds(1f);
        }

        IsSelecting[player] = false;

        if (!Selections.ContainsKey(player)) yield break;

        if (abilities.All(x => x == abilities.First()))
        {
            for (var i = 0; i < 3; i++)
            {
                player.AddAbility(abilities[i]);
            }

            Selections.Remove(player);

            yield break;
        }

        var random = Random.Range(0, 3);

        player.AddAbility(abilities[random]);

        Selections.Remove(player);
    }

    private AbilityCategory GetCategory(Player player)
    {
        if (!player.IsAlive) return AbilityCategory.None;

        if (player.Role == RoleTypeId.Scp079)
            return AbilityCategory.Scp079;

        var random = Random.Range(1, 1001);

        switch (random)
        {
            case <= 1:
                return AbilityCategory.Mythic;
            case <= 5:
                return AbilityCategory.Legend;
            case <= 55:
                return AbilityCategory.Epic;
            case <= 295:
                return AbilityCategory.Rare;
            default:
                return AbilityCategory.Common;
        }
    }

    public bool Select(Player player, int index, out string response)
    {
        if (!Selections.ContainsKey(player))
        {
            response = "선택할 수 있는 능력이 없습니다.";
            return false;
        }

        if (index is < 1 or > 3)
        {
            response = "1 ~ 3 사이의 숫자를 입력해주세요.";
            return false;
        }

        Log.Info("Select called with " + player.Nickname + " and " + index);

        AbilityType ability;
        if (Selections[player].All(x => x == Selections[player].First()))
        {
            Log.Info("All abilities are the same");

            ability = Selections[player].First();

            for (var i = 0; i < 3; i++)
            {
                player.AddAbility(ability);
            }

            Selections.Remove(player);

            player.ShowHint("", 0.1f);

            response = $"{index}번 능력 선택 완료!";
            return true;
        }

        Log.Info("All abilities are not the same");

        ability = Selections[player][index - 1];

        player.AddAbility(ability);

        Selections.Remove(player);

        player.ShowHint("", 0.1f);

        response = $"{index}번 능력 선택 완료!";
        return true;
    }
}

public static class ABattleExtensions
{
    public static void AddAbility(this Player player, AbilityType type)
    {
        ABattle.Instance.AddAbility(player, type);
    }

    public static void RemoveAbility(this Player player, AbilityType type)
    {
        ABattle.Instance.RemoveAbility(player, type);
    }

    public static void RemoveAbility(this Player player, Ability ability)
    {
        ABattle.Instance.RemoveAbility(player, ability);
    }

    public static void RemoveAllAbilities(this Player player)
    {
        ABattle.Instance.RemoveAllAbilities(player);
    }

    public static List<Ability> GetAbilities(this Player player)
    {
        return ABattle.Instance.GetAbilities(player);
    }

    public static Ability GetAbility(this Player player, AbilityType type)
    {
        return ABattle.Instance.GetAbility(player, type);
    }

    public static bool HasAbility(this Player player, AbilityType type)
    {
        return ABattle.Instance.HasAbility(player, type);
    }
}

public static class Utilities
{
    public static void AddEffect(this Player player, EffectType type, byte intensity, float duration = 0f, bool addDuration = false)
    {
        var effects = player.ActiveEffects.Select(x => x.GetEffectType()).ToList();

        if (effects.Contains(type))
        {
            var effect = player.ActiveEffects.First(x => x.GetEffectType() == type);

            effect.ServerChangeDuration(duration, addDuration);

            if (effect.Intensity + intensity > 255)
                effect.Intensity = 255;
            else
                effect.Intensity += intensity;
        }
        else
        {
            player.EnableEffect(type, intensity, duration);
        }
    }

    public static void RemoveEffect(this Player player, EffectType type, byte intensity)
    {
        if (player.ActiveEffects.Any(x => x.GetEffectType() == type))
        {
            var effect = player.ActiveEffects.First(x => x.GetEffectType() == type);

            if (effect.Intensity - intensity <= 0)
                player.DisableEffect(type);
            else
                effect.Intensity -= intensity;
        }
    }
}