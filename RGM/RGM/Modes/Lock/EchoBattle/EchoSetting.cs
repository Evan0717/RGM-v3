using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace RGM.RGM.Modes.Lock.EchoBattle;

public static class EchoSetting
{
    const int SettingIdStart = 3100;
    const int InfoKeyId = 31001001;

    static readonly Dictionary<int, EchoSlotKind> SlotMeta = new();
    static readonly List<int> SlotSettingIds = new();

    public static HeaderSetting Header { get; private set; } = new HeaderSetting(190031, "에코 전투");
    public static KeybindSetting InfoKey { get; private set; }

    enum EchoSlotKind
    {
        Main,
        Sub0,
        Sub1,
        Sub2,
        Sub3
    }

    public static void Init()
    {
        SlotMeta.Clear();
        SlotSettingIds.Clear();

        var list = new List<SettingBase>();

        InfoKey = new KeybindSetting(
            id: InfoKeyId,
            label: "<b>에코 전투 정보 키</b>",
            suggested: KeyCode.F1,
            preventInteractionOnGUI: true,
            hintDescription: "힌트 표시를 토글합니다.",
            header: Header
        );
        list.Add(InfoKey);

        int nextId = SettingIdStart;
        string noneOption = "없음";

        void addSlot(string label, EchoSlotKind kind, EchoCost? filterCost = null)
        {
            int id = nextId++;
            var options = BuildOptions(filterCost).Prepend(noneOption).ToList();
            list.Add(new DropdownSetting(
                id: id,
                label: label,
                hintDescription: BuildHintDescription(filterCost),
                options: options,
                header: Header
            ));
            SlotMeta[id] = kind;
            SlotSettingIds.Add(id);
        }

        addSlot("<color=#f43838>메인 Echo</color>", EchoSlotKind.Main);
        addSlot("<color=#88aaff>부가 Echo 1</color>", EchoSlotKind.Sub0);
        addSlot("<color=#88aaff>부가 Echo 2</color>", EchoSlotKind.Sub1);
        addSlot("<color=#88aaff>부가 Echo 3</color>", EchoSlotKind.Sub2);
        addSlot("<color=#88aaff>부가 Echo 4</color>", EchoSlotKind.Sub3);

        SettingBase.Register(list);
    }

    static IEnumerable<string> BuildOptions(EchoCost? filterCost)
    {
        foreach (var pair in EchoInfo.Echoes.OrderByDescending(x => (int)x.Value.Cost).ThenBy(x => x.Value.Name))
        {
            if (filterCost.HasValue && pair.Value.Cost != filterCost.Value)
                continue;

            yield return FormatOption(pair.Value);
        }
    }

    static string BuildHintDescription(EchoCost? filterCost)
    {
        var lines = EchoInfo.Echoes.Values
            .Where(x => !filterCost.HasValue || x.Cost == filterCost.Value)
            .OrderByDescending(x => (int)x.Cost)
            .Select(x => $"• {FormatOption(x)}: {x.Description}");

        return string.Join("\n", lines) + $"\n\n최대 {EchoInfo.MaxEquippedEchoes}개 / 합산 Cost {EchoInfo.MaxTotalCost}";
    }

    static string FormatOption(EchoData data)
    {
        return $"<color={data.Cost.GetColor()}>[C{(int)data.Cost}]</color> {data.Emoji} {data.Name}";
    }

    public static void OnSSInput(ReferenceHub sender, ServerSpecificSettingBase setting)
    {
        Player player = Player.Get(sender);
        if (player == null)
            return;

        if (setting is SSKeybindSetting keybind && keybind.SyncIsPressed && keybind.SettingId == InfoKeyId)
        {
            if (!EchoInfo.PlayerShowHints.ContainsKey(player))
                EchoInfo.PlayerShowHints[player] = true;

            EchoInfo.PlayerShowHints[player] = !EchoInfo.PlayerShowHints[player];
            return;
        }

        if (setting is not SSDropdownSetting dropdown)
            return;

        if (!SlotMeta.TryGetValue(setting.SettingId, out var slotKind))
            return;

        string selected = dropdown.SyncSelectionText;
        if (string.IsNullOrWhiteSpace(selected))
            return;

        if (!EchoInfo.PlayerLoadouts.ContainsKey(player))
            EchoInfo.PlayerLoadouts[player] = new EchoLoadout();

        var loadout = EchoInfo.PlayerLoadouts[player];
        EchoType? selectedType = null;

        if (selected != "없음")
        {
            var match = EchoInfo.Echoes.Values.FirstOrDefault(x => FormatOption(x) == selected);
            if (match == null)
                return;

            selectedType = match.EchoType;

            // 같은 이름 Echo 중복 장착 불가
            if (loadout.Contains(selectedType.Value) && GetCurrentSlotType(loadout, slotKind) != selectedType)
            {
                player.ShowHint("<color=red>같은 Echo는 중복 장착할 수 없습니다.</color>", 3);
                return;
            }
        }

        // 임시 적용 후 cost/개수 검증
        var previous = GetCurrentSlotType(loadout, slotKind);
        SetSlot(loadout, slotKind, selectedType);

        if (loadout.GetEquippedCount() > EchoInfo.MaxEquippedEchoes
            || loadout.GetTotalCost() > EchoInfo.MaxTotalCost)
        {
            SetSlot(loadout, slotKind, previous);
            player.ShowHint($"<color=red>제한 초과</color> (최대 {EchoInfo.MaxEquippedEchoes}개 / Cost {EchoInfo.MaxTotalCost})", 3);
            return;
        }

        int cost = loadout.GetTotalCost();
        int count = loadout.GetEquippedCount();
        player.ShowHint($"Echo 장착: {count}/{EchoInfo.MaxEquippedEchoes} | Cost {cost}/{EchoInfo.MaxTotalCost}", 2);
    }

    static EchoType? GetCurrentSlotType(EchoLoadout loadout, EchoSlotKind kind)
    {
        return kind switch
        {
            EchoSlotKind.Main => loadout.MainSlot,
            EchoSlotKind.Sub0 => loadout.SubSlots[0],
            EchoSlotKind.Sub1 => loadout.SubSlots[1],
            EchoSlotKind.Sub2 => loadout.SubSlots[2],
            EchoSlotKind.Sub3 => loadout.SubSlots[3],
            _ => null
        };
    }

    static void SetSlot(EchoLoadout loadout, EchoSlotKind kind, EchoType? type)
    {
        switch (kind)
        {
            case EchoSlotKind.Main:
                loadout.MainSlot = type;
                break;
            case EchoSlotKind.Sub0:
                loadout.SubSlots[0] = type;
                break;
            case EchoSlotKind.Sub1:
                loadout.SubSlots[1] = type;
                break;
            case EchoSlotKind.Sub2:
                loadout.SubSlots[2] = type;
                break;
            case EchoSlotKind.Sub3:
                loadout.SubSlots[3] = type;
                break;
        }
    }
}