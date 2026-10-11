using System;
using System.Collections.Generic;
using System.Reflection;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using RGM.API.Features;
using UnityEngine;
using MEC;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features.Pickups;
using Exiled.API.Enums;
using PlayerRoles;

namespace RGM.Modes;

[Mode(ModeCategory.Private, ModeInfo.Lock, ModeType.Minecraft)]
public class Minecraft : Mode
{
    public override string Name => "마인크래프트";
    public override string Description => "A mode that turns the map into a Minecraft world.";
    public override string Detail =>
        """
        [ESC] -> [Settings] -> [Server-specific] -> 상호작용, 설치, 파괴 키를 할당하세요.
        블록은 설치할 수 있으며, 도구는 파괴 속도에 영향을 줍니다.
        """;
    public override string Color => "#00FF00";
    public override string Author => "Ragdoll";

    public static Minecraft Instance;

    public const float GridSize = 1f;
    public const float MaxDistance = 5f;
    public const int BuildMask = 1 << 0;
    public readonly ItemType MinecraftItemType = ItemType.KeycardJanitor; // 블록 설치용 아이템
    public readonly Dictionary<BlockType, BlockInfo> Blocks = new();
    public readonly Dictionary<Vector3Int, Block> World = new();
    public readonly Dictionary<ushort, BlockData> BlockSerials = new();
    private MinecraftEventHandler _eventHandler;
    private CoroutineHandle _startHandle;

    // Lifecycle
    public override void OnEnabled()
    {
        Instance = this;
        _eventHandler = new MinecraftEventHandler(this);
        Blocks.Clear();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var blockAttribute = type.GetCustomAttribute<BlockAttribute>();
            if (blockAttribute == null)
                continue;

            if (!typeof(Block).IsAssignableFrom(type))
                continue;

            if (Blocks.TryGetValue(blockAttribute.blockType, out var existing))
            {
                Log.Error(
                    $"Duplicate BlockType '{blockAttribute.blockType}' on {type.FullName}. Already registered by {existing.Type.FullName}.");
                continue;
            }

            Blocks.Add(blockAttribute.blockType, new BlockInfo
            {
                Type = type,
                Name = blockAttribute.Name,
                SchematicName = blockAttribute.SchematicName,
                BlockType = blockAttribute.blockType,
                DestroyTime = blockAttribute.DestroyTime
            });
        }
        _startHandle = Timing.RunCoroutine(OnModeStarted());
        _eventHandler.RegisterEvents();
        MinecraftSetting.Register();
    }

    public override void OnDisabled()
    {
        _eventHandler.UnRegisterEvents();   // 이벤트를 먼저 끊고
        Timing.KillCoroutines(_startHandle);

        foreach (var block in World.Values)
            block.Schematic?.Destroy();
        World.Clear();

        foreach (var player in Player.List)
            foreach (var item in player.Items.Where(i => BlockSerials.ContainsKey(i.Serial)).ToList())
                player.RemoveItem(item);

        foreach (var pickup in Pickup.List.Where(p => BlockSerials.ContainsKey(p.Serial)).ToList())
            pickup.Destroy(); // Dropped 스키매틱도 같이 사라짐

        BlockSerials.Clear();
        Blocks.Clear();
    }

    // Startup coroutine
    private IEnumerator<float> OnModeStarted()
    {
        yield return Timing.WaitForSeconds(1f);
        foreach (var player in PlayerManager.List.Where(p => p != null && p.IsAlive))
        {
            Spawned(player);
            player.AddHint("경고", 
                $"""
                [ESC] -> [Settings] -> [Server-specific] -> 상호작용, 설치, 파괴 키를 할당하세요.
                <color=#ff0000>블록을 이용한 맵 탈출, 아군 세력간 트롤은 제재될 수 있습니다.
                플레이어 위치에 블록을 설치하면 튕겨나올 수 있으니 가급적이면 자제하세요.</color>
                """,
                10f);
        }
    }

    // Spawn / Inventory API
    public void Spawned(Player player)
    {
        BlockType RandomBlockType = Tools.EnumToList<BlockType>().Where(t => t != BlockType.Air).GetRandomValue();
        AddBlock(RandomBlockType, (ushort)32, player);

        player.EnableEffect(EffectType.Lightweight, 30);
        if (player.Role == RoleTypeId.Scp096)
            player.EnableEffect(EffectType.Lightweight, 40);
    }

    public void AddBlock(BlockType type, ushort count, Player player)
    {
        Item item = player.AddItem(MinecraftItemType);
        ushort ItemSerial = item.Serial;

        BlockSerials.Add(ItemSerial, new BlockData { Type = type, Count = count });
    }

    // Placement / Interaction API
    public bool TryPlaceBlock(Player player, BlockType type)
    {
        if (!Blocks.TryGetValue(type, out var info))
            return false;

        Transform cam = player.CameraTransform;
        if (!Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, MaxDistance, BuildMask))
            return false;

        Vector3Int grid = ToGrid(hit.point + hit.normal * (GridSize * 0.05f));

        if (World.ContainsKey(grid))
            return false;

        var visual = new BlockVisual(info, BlockForm.Placed, ToWorld(grid));
        if (visual.Schematic == null)
        {
            Log.Error($"Schematic '{info.SchematicName}' not found.");
            return false;
        }

        var block = (Block)Activator.CreateInstance(info.Type);
        block.Info = info;
        block.GridPos = grid;
        block.Schematic = visual.Schematic;

        World[grid] = block;
        block.OnPlaced(player, hit);
        _eventHandler.SwingHeld(player);
        return true;
    }

    public bool TryPlaceHeldBlock(Player player)
    {
        if (player.CurrentItem is not { } item || !BlockSerials.TryGetValue(item.Serial, out BlockData data))
            return false;

        if (!TryPlaceBlock(player, data.Type))
            return false;

        if (--data.Count <= 0)
        {
            BlockSerials.Remove(item.Serial);
            player.RemoveItem(item); // 개수가 0이면 아이템 제거
        }

        _eventHandler.SwingHeld(player);
        return true;
    }

    public bool TryInteract(Player player)
    {
        if (!TryGetLookedBlock(player, out Block block))
            return false;

        if (!block.OnInteract(player))
            return false;

        _eventHandler.SwingHeld(player);
        return true;
    }

    public bool TryDestroyBlock(Player player)
    {
        if (!TryGetLookedBlock(player, out Block block))
            return false;

        block.OnBroken(player);
        block.Schematic?.Destroy();
        World.Remove(block.GridPos);
        _eventHandler.SwingHeld(player);

        try
        {
            Pickup pickup = Pickup.CreateAndSpawn(MinecraftItemType, ToWorld(block.GridPos));
            if (pickup != null)
                BlockSerials[pickup.Serial] = new BlockData { Type = block.Info.BlockType, Count = 1 };
            var visual = new BlockVisual(block.Info, BlockForm.Dropped, pickup.Transform.position);
            visual.AttachTo(pickup.Transform);
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to spawn block pickup: {ex}");
        }

        return true;
    }

    // Helpers
    private bool TryGetLookedBlock(Player player, out Block block)
    {
        block = null;

        Transform cam = player.CameraTransform;
        if (!Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, MaxDistance, BuildMask))
            return false;

        Vector3Int grid = ToGrid(hit.point - hit.normal * (GridSize * 0.5f));
        return World.TryGetValue(grid, out block);
    }

    private static Vector3Int ToGrid(Vector3 pos) => new(
        Mathf.RoundToInt(pos.x / GridSize),
        Mathf.RoundToInt(pos.y / GridSize),
        Mathf.RoundToInt(pos.z / GridSize));

    private static Vector3 ToWorld(Vector3Int grid) => new(
        grid.x * GridSize,
        grid.y * GridSize,
        grid.z * GridSize);
}