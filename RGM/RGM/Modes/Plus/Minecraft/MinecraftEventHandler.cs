using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using RGM.API.Features;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes;

public class MinecraftEventHandler(Minecraft minecraft) 
{
    private readonly Dictionary<Player, BlockVisual> _held = new();

    internal void RegisterEvents()
    {
        Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
        Exiled.Events.Handlers.Player.DroppedItem += OnDroppedItem;
        Exiled.Events.Handlers.Player.ItemAdded += OnItemAdded;
        Exiled.Events.Handlers.Player.Left += OnLeft;
        Exiled.Events.Handlers.Player.Died += OnDied;
    }

    internal void UnRegisterEvents()
    {
        Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
        Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;
        Exiled.Events.Handlers.Player.DroppedItem -= OnDroppedItem;
        Exiled.Events.Handlers.Player.ItemAdded -= OnItemAdded;
        Exiled.Events.Handlers.Player.Left -= OnLeft;
        Exiled.Events.Handlers.Player.Died -= OnDied;

        foreach (var visual in _held.Values.ToList())
            visual.Destroy();
        _held.Clear();
    }

    private void OnSpawned(SpawnedEventArgs ev)
    {
        if (ev.Player == null || ev.Player.IsNPC || !ev.Player.IsAlive)
            return;
        minecraft.Spawned(ev.Player);
    }

    private void OnChangedItem(ChangedItemEventArgs ev)
    {
        RemoveHeld(ev.Player);

        if (ev.Item == null
    || !minecraft.BlockSerials.TryGetValue(ev.Item.Serial, out var data)
    || !minecraft.Blocks.TryGetValue(data.Type, out var info))
            return;

        var visual = new BlockVisual(info, BlockForm.Held, ev.Player.Position);
        visual.AttachTo(ev.Player.Transform);
        _held[ev.Player] = visual;

        ev.Player.AddHint("minecraft_held", $"<size=25>{minecraft.Blocks[data.Type].Name} x{data.Count}</size>", 3f);
    }

    private void OnDroppedItem(DroppedItemEventArgs ev)
    {
        if (!minecraft.BlockSerials.TryGetValue(ev.Pickup.Serial, out var data))
            return;
        if (ev.Player.CurrentItem == null)
            RemoveHeld(ev.Player);
        var visual = new BlockVisual(minecraft.Blocks[data.Type], BlockForm.Dropped, ev.Pickup.Position);
        visual.AttachTo(ev.Pickup.Transform);
    }

    private void OnItemAdded(ItemAddedEventArgs ev)
    {
        if (!minecraft.BlockSerials.TryGetValue(ev.Item.Serial, out BlockData picked))
            return;

        BlockData owned = null;
        foreach (var item in ev.Player.Items)
        {
            if (item.Serial == ev.Item.Serial)
                continue;

            if (minecraft.BlockSerials.TryGetValue(item.Serial, out BlockData data) && data.Type == picked.Type)
            {
                owned = data;
                break;
            }
        }

        if (owned == null)
            return;

        owned.Count += picked.Count;
        minecraft.BlockSerials.Remove(ev.Item.Serial);
        ev.Player.RemoveItem(ev.Item);

        /*if (ev.Player.CurrentItem is { } cur && minecraft.BlockSerials.TryGetValue(cur.Serial, out var curData))
            ev.Player.AddHint("minecraft_held", $"{minecraft.Blocks[curData.Type].Name} x{curData.Count}", 3f);*/
    }

    private void OnLeft(LeftEventArgs ev) => RemoveHeld(ev.Player);
    private void OnDied(DiedEventArgs ev) => RemoveHeld(ev.Player);

    public void SwingHeld(Player player)
    {
        if (_held.TryGetValue(player, out var visual))
            visual.PlaySwing();
    }

    private void RemoveHeld(Player player)
    {
        if (player != null && _held.TryGetValue(player, out var visual))
        {
            visual.Destroy();
            _held.Remove(player);
        }
    }
}
