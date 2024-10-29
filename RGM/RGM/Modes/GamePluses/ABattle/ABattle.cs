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
using Random = UnityEngine.Random;

namespace RGM.Modes;

public class ABattle
{
    public static ABattle Instance { get; set; }

    public Dictionary<Player, List<WorkstationController>> PlayerWorkstations { get; set; }
    public Dictionary<Player, List<Ability>> PlayerAbilities { get; set; }
    public Dictionary<AbilityType, AbilityData> Abilities { get; set; }

    private Dictionary<AbilityType, List<AbilityType>> _synergyAbilities;
    private Dictionary<Player, List<AbilityType>> _selections;
    private bool _isFeverModeEnabled;

    private ABattleEventHandler _eventHandler;

    // 플러그인에 있는 모든 능력 검색
    public void OnEnabled()
    {
        Instance = this;

        _eventHandler = new ABattleEventHandler(this);
        _eventHandler.RegisterEvents();

        _synergyAbilities = new Dictionary<AbilityType, List<AbilityType>>();
        _selections = new Dictionary<Player, List<AbilityType>>();

        PlayerWorkstations = new Dictionary<Player, List<WorkstationController>>();
        PlayerAbilities = new Dictionary<Player, List<Ability>>();
        Abilities = new Dictionary<AbilityType, AbilityData>();

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
                _synergyAbilities.Add(abilityAttribute.Type, requiresAbilityAttribute.Abilities.ToList());

                Abilities[abilityAttribute.Type].Category = AbilityCategory.Synergy;
                Abilities[abilityAttribute.Type].Requires = requiresAbilityAttribute.Abilities.ToList();
            }
        }

        QueryProcessor.DotCommandHandler.RegisterCommand(new VoteFirst());
        QueryProcessor.DotCommandHandler.RegisterCommand(new VoteSecond());
        QueryProcessor.DotCommandHandler.RegisterCommand(new VoteThird());

        CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(new AddAbility());

        Timing.CallDelayed(Random.Range(1, 11), () =>
        {
            if (Random.Range(1, 6) == 1)
                _isFeverModeEnabled = true;

            if (_isFeverModeEnabled)
                Server.ExecuteCommand("/mp load ABattle");
        });

        foreach (var player in Player.List)
        {
            PlayerWorkstations.Add(player, new List<WorkstationController>());
            PlayerAbilities.Add(player, new List<Ability>());
        }

        Timing.RunCoroutine(HintCoroutine());
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
        if (!PlayerAbilities[player].Any())
        {
            return player.Role.Type == RoleTypeId.Scp079
                ? "<align=left><b><size=22>레벨이 오를 때마다 능력을 획득할 수 있습니다.</size></b></align>"
                : "<align=left><b><size=22>워크스테이션 위에서 점프하면 능력을 획득할 수 있습니다.</size></b></align>";
        }

        var abilitiesText = string.Join(", ",
            PlayerAbilities[player]
                .GroupBy(x => x)
                .Select(g => g.Count() > 1
                    ? $"{g.Key.Data.GetFormattedName()} ({g.Count()})"
                    : g.Key.Data.GetFormattedName())
                .ToList());

        return $"<align=left><b><size=25>보유 업그레이드</size></b>\n<size=20>{abilitiesText}</size></align>";
    }

    // 플레이어에게 특정 능력을 부여
    public void AddAbility(Player player, AbilityType type)
    {
        if (!Abilities.ContainsKey(type))
            return;

        if (!PlayerAbilities.ContainsKey(player))
            PlayerAbilities.Add(player, []);

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
            return;

        ability.Data = abilityData;
        ability.Owner = player;
        ability.OnEnabled();

        PlayerAbilities[player].Add(ability);

        EnableSynergyAbility(PlayerAbilities[player]);
    }

    // 플레이어에게 시너지 능력 부여
    private void EnableSynergyAbility(List<Ability> abilities)
    {
        foreach (var synergy in _synergyAbilities.Where(synergy =>
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
        foreach (var synergy in _synergyAbilities)
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
        if (!_selections.ContainsKey(player))
            _selections.Add(player, new List<AbilityType>());

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

        _selections[player] = abilities;

        // 다음 타자, 코루틴!!!
        Timing.RunCoroutine(SelectionCoroutine(player));
    }

    private IEnumerator<float> SelectionCoroutine(Player player)
    {
        var abilities = _selections[player];
        var text = string.Join("\n", abilities.Select((x, i) => $"[{i + 1}] {x.GetTranslation()}"));

        for (var i = 0; i < 20; i++)
        {
            if (player.IsDead) yield break;
            if (!_selections.ContainsKey(player)) yield break;

            player.ShowHint(
                $"<align=left><size=30>{text}</size>\n\n<size=25><b>{20 - i}초 안에 [.(번호)] 명령어로 원하는 능력을 선택하세요. (ex .1)</b></size></align>\n\n",
                1.2f);

            yield return Timing.WaitForSeconds(1f);
        }

        if (!_selections.ContainsKey(player)) yield break;

        if (abilities.All(x => x == abilities.First()))
        {
            for (var i = 0; i < 3; i++)
            {
                player.AddAbility(abilities[i]);
            }

            _selections.Remove(player);

            yield break;
        }

        var random = Random.Range(0, 3);

        player.AddAbility(abilities[random]);

        _selections.Remove(player);
    }

    private AbilityCategory GetCategory(Player player)
    {
        if (!player.IsAlive) return AbilityCategory.None;

        if (player.Role == RoleTypeId.Scp079)
            return AbilityCategory.Scp079;

        var random = Random.Range(1, 10001);

        switch (random)
        {
            case <= 5:
                return AbilityCategory.Mythic;
            case <= 40:
                return AbilityCategory.Legend;
            case <= 250:
                return AbilityCategory.Epic;
            case <= 1500:
                return AbilityCategory.Rare;
            case <= 2000 when player.Role.Type.ToString().StartsWith("Scp"):
                return (AbilityCategory)Enum.Parse(typeof(AbilityCategory), player.Role.Type.ToString());
            case <= 2000:
                switch (player.Role.Type)
                {
                    case RoleTypeId.ClassD:
                        return AbilityCategory.ClassD;
                    case RoleTypeId.Scientist:
                        return AbilityCategory.Scientist;
                    case RoleTypeId.NtfSpecialist:
                    case RoleTypeId.NtfSergeant:
                    case RoleTypeId.NtfCaptain:
                    case RoleTypeId.NtfPrivate:
                        return AbilityCategory.Ntf;
                    case RoleTypeId.ChaosConscript:
                    case RoleTypeId.ChaosRifleman:
                    case RoleTypeId.ChaosMarauder:
                    case RoleTypeId.ChaosRepressor:
                        return AbilityCategory.Chaos;
                    case RoleTypeId.FacilityGuard:
                        return AbilityCategory.Guard;
                    case RoleTypeId.Tutorial:
                        return AbilityCategory.Snake;
                    case RoleTypeId.None:
                    case RoleTypeId.Scp173:
                    case RoleTypeId.Spectator:
                    case RoleTypeId.Scp106:
                    case RoleTypeId.Scp049:
                    case RoleTypeId.Scp079:
                    case RoleTypeId.Scp096:
                    case RoleTypeId.Scp0492:
                    case RoleTypeId.Scp939:
                    case RoleTypeId.CustomRole:
                    case RoleTypeId.Overwatch:
                    case RoleTypeId.Filmmaker:
                    case RoleTypeId.Scp3114:
                    default:
                        return AbilityCategory.Common;
                }

            default:
                return AbilityCategory.Common;
        }
    }

    public bool Select(Player player, int index, out string response)
    {
        if (!_selections.ContainsKey(player))
        {
            response = "선택할 수 있는 능력이 없습니다.";
            return false;
        }

        if (index is < 1 or > 3)
        {
            response = "1 ~ 3 사이의 숫자를 입력해주세요.";
            return false;
        }

        AbilityType ability;
        if (_selections[player].All(x => x == _selections[player].First()))
        {
            ability = _selections[player].First();

            for (var i = 0; i < 3; i++)
            {
                player.AddAbility(ability);
            }

            _selections.Remove(player);

            player.ShowHint("", 0.1f);

            response = $"{index}번 능력 선택 완료!";
            return true;
        }

        ability = _selections[player][index - 1];

        player.AddAbility(ability);

        _selections.Remove(player);

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