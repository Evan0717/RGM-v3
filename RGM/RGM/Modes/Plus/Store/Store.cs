using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using MapGeneration.Holidays;
using MEC;

using PlayerRoles;
using RGM.API.Features;
using UserSettings.ServerSpecific;
using static RGM.Variables.Variable;
using RGM.Modes.Plus.Store;

namespace RGM.Modes;

[Mode(ModeCategory.Private, ModeInfo.Plus, ModeType.Store)]  
public class Store : Mode
{
    public override string Name => "스토어";
    public override string Description => "상점에서 아이템을 구매하여 강해지세요.";
    public override string Detail =>
"""
아잉만들기귀찮앙
""";
    public override string Color => "e8ed7c";
    public override string Map => "Store";

    public static Store Instance;

    public Dictionary<StoreItemType, StoreItemData> StoreItems = new Dictionary<StoreItemType, StoreItemData>();
    public Dictionary<Player, List<StoreItem>> PlayerStoreItems = new Dictionary<Player, List<StoreItem>>();
    public Dictionary<Player, int> SelectionCursor = new Dictionary<Player, int>();

    private StoreEventHandler _eventHandler;

    CoroutineHandle _onModeStarted;
    CoroutineHandle _hintCoroutine;

    public override void OnEnabled()
    {
        Instance = this;

        _eventHandler = new StoreEventHandler(this);
        _eventHandler.RegisterEvents();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var StoreItemAttribute = type.GetCustomAttribute<StoreItemAttribute>();

            if (StoreItemAttribute == null)
                continue;

            if (!typeof(StoreItem).IsAssignableFrom(type))
                continue;

            if (StoreItemAttribute.HolidayType == StoreItemHolidayType.Christmas && !HolidayUtils.IsHolidayActive(HolidayType.Christmas))
                continue;

            if (StoreItemAttribute.HolidayType == StoreItemHolidayType.Halloween && !HolidayUtils.IsHolidayActive(HolidayType.Halloween))
                continue;

            StoreItems.Add(StoreItemAttribute.Type, new StoreItemData
            {
                Type = type,
                Name = StoreItemAttribute.Name,
                Description = StoreItemAttribute.Description,
                Level = StoreItemAttribute.Level,
                StoreItemType = StoreItemAttribute.Type,
                HolidayType = StoreItemAttribute.HolidayType,
            });
        }

        _onModeStarted = Timing.RunCoroutine(OnModeStarted());
        _hintCoroutine = Timing.RunCoroutine(HintCoroutine());

        ServerSpecificSettingsSync.ServerOnSettingValueReceived += StoreSetting.OnSSInput;
    }

    public override void OnDisabled()
    {
        _eventHandler.UnregisterEvents();

        foreach (var player in Player.List)
        {
            foreach (var StoreItem in GetStoreItems(player))
            {
                StoreItem.OnDisabled();
            }
        }

        Timing.KillCoroutines(_onModeStarted);
        Timing.KillCoroutines(_hintCoroutine);

        ServerSpecificSettingsSync.ServerOnSettingValueReceived -= StoreSetting.OnSSInput;
    }

    private IEnumerator<float> OnModeStarted()
    {
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

                if (player.IsAlive)
                    player.AddHint("워크스테이션 힌트", FormatHint(player), 1.2f);
            }

            yield return Timing.WaitForOneFrame;
        }
    }

    private string FormatHint(Player player)
    {
        if (!PlayerStoreItems.TryGetValue(player, out var StoreItem))
        {
            return player.Role.Type == RoleTypeId.Scp079
                ? "<align=left><b><size=22>워크스테이션 상단을 핑으로 찍으면 능력을 획득할 수 있습니다.</size></b></align>"
                : "<align=left><b><size=22>워크스테이션 위에서 점프하면 능력을 획득할 수 있습니다.</size></b></align>";
        }

        if (!StoreItem.Any())
        {
            return player.Role.Type == RoleTypeId.Scp079
                ? "<align=left><b><size=22>워크스테이션 상단을 핑으로 찍으면 능력을 획득할 수 있습니다.</size></b></align>"
                : "<align=left><b><size=22>워크스테이션 위에서 점프하면 능력을 획득할 수 있습니다.</size></b></align>";
        }

        var StoreItemsText = string.Join(", ",
            PlayerStoreItems[player]
                .GroupBy(x => x.Data.StoreItemType)
                .Select(g => g.Count() > 1
                    ? $"{g.First().Data.GetFormattedName()} x{g.Count()}"
                    : g.First().Data.GetFormattedName())
                .ToList());

        return $"<align=left><b><size=25>보유 업그레이드</size></b>\n<size=20>{StoreItemsText}</size>\n</align>";
    }

    public IEnumerator<float> RestoreStoreItems(List<Player> players)
    {
        foreach (var player in players)
        {
            List<StoreItemType> _StoreItems = PlayerStoreItems[player].Select(x => x.Data.StoreItemType).ToList();

            Reset(player);

            yield return Timing.WaitForOneFrame;

            foreach (var StoreItem in _StoreItems)
                player.AddStoreItem(StoreItem);

            yield return Timing.WaitForOneFrame;

            player.AddBroadcast(10, $"<size=25><b>모든 능력을 제거한 후, 수복하였습니다.</b></size>");
        }
    }

    public void AddStoreItem(Player player, StoreItemType type)
    {
        Log.Info("AddStoreItem called with " + player.Nickname + " and " + type);

        if (!StoreItems.ContainsKey(type))
        {
            Log.Error($"StoreItem {type} not found.");

            return;
        }

        if (!PlayerStoreItems.ContainsKey(player))
        {
            Log.Info("No key");
            PlayerStoreItems.Add(player, []);
        }

        var StoreItemData = StoreItems[type];
        StoreItem StoreItem;

        try
        {
             StoreItem = Activator.CreateInstance(StoreItems[type].Type) as StoreItem;
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred while trying to create an instance of {StoreItemData.Name}: {e}");
            return;
        }

        if (StoreItem == null)
        {
            Log.Error($"An error occurred while trying to create an instance of {StoreItemData.Name}. The instance is null.");
            return;
        }

        StoreItem.Data = StoreItemData;
        StoreItem.Owner = player;
        StoreItem.OnEnabled();

        PlayerStoreItems[player].Add(StoreItem);
    }

    public void RemoveStoreItem(Player player, StoreItemType type)
    {
        if (!PlayerStoreItems.TryGetValue(player, out var playerStoreItem))
            return;

        var StoreItem = playerStoreItem.FirstOrDefault(x => x.Data.StoreItemType == type);

        if (StoreItem == null)
            return;

        StoreItem.OnDisabled();
        PlayerStoreItems[player].Remove(StoreItem);
    }

    public void RemoveStoreItem(Player player, StoreItem StoreItem)
    {
        if (StoreItem == null)
            return;

        if (!PlayerStoreItems.TryGetValue(player, out var playerStoreItem))
            return;

        if (!playerStoreItem.Contains(StoreItem))
            return;

        StoreItem.OnDisabled();
        PlayerStoreItems[player].Remove(StoreItem);
    }

    public void RemoveAllStoreItems(Player player)
    {
        if (!PlayerStoreItems.TryGetValue(player, out var playerStoreItem))
            return;

        foreach (var StoreItem in playerStoreItem)
            StoreItem.OnDisabled();

        PlayerStoreItems.Remove(player);
    }

    public List<StoreItem> GetStoreItems(Player player)
    {
        return PlayerStoreItems.TryGetValue(player, out var playerStoreItem) ? playerStoreItem : new List<StoreItem>();
    }

    public StoreItemType FindStoreItem(string name)
    {
        return StoreItems.FirstOrDefault(x => x.Value.Name == name).Key;
    }
    public StoreItem GetStoreItem(Player player, StoreItemType type)
    {
        return GetStoreItems(player).FirstOrDefault(x => x.Data.StoreItemType == type);
    }

    public bool HasStoreItem(Player player, StoreItemType type)
    {
        return GetStoreItem(player, type) != null;
    }

    public void MoveSelectionCursor(Player player, int delta)
    {
        if (!SelectionCursor.ContainsKey(player))
            SelectionCursor[player] = 0;

        int cursor = SelectionCursor[player];
        cursor = (cursor + delta) % StoreItems.Count;

        if (cursor < 0)
            cursor += StoreItems.Count;

        SelectionCursor[player] = cursor;

        PlayersAudio[player].TryPlay("Select");
    }

    public bool ConfirmSelectionByCursor(Player player, out string response)
    {
        if (!SelectionCursor.ContainsKey(player))
            SelectionCursor[player] = 0;

        PlayersAudio[player].TryPlay("SelectConfirm", 1.5f);

        int cursor = Math.Max(0, Math.Min(SelectionCursor[player], StoreItems.Count - 1));
        response = "임시";
        return true;
    }

    public void Reset(Player player)
    {
        player.RemoveAllStoreItems();

        SelectionCursor.Remove(player);
    }
}

public static class StoreExtensions
{
    public static void AddStoreItem(this Player player, StoreItemType type)
    {
        Store.Instance.AddStoreItem(player, type);
    }

    public static void RemoveStoreItem(this Player player, StoreItemType type)
    {
        Store.Instance.RemoveStoreItem(player, type);
    }

    public static void RemoveStoreItem(this Player player, StoreItem StoreItem)
    {
        Store.Instance.RemoveStoreItem(player, StoreItem);
    }

    public static void RemoveAllStoreItems(this Player player)
    {
        Store.Instance.RemoveAllStoreItems(player);
    }

    public static List<StoreItem> GetStoreItems(this Player player)
    {
        return Store.Instance.GetStoreItems(player);
    }

    public static StoreItem GetStoreItem(this Player player, StoreItemType type)
    {
        return Store.Instance.GetStoreItem(player, type);
    }

    public static bool HasStoreItem(this Player player, StoreItemType type)
    {
        return Store.Instance.HasStoreItem(player, type);
    }
}