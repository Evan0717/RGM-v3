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

    private const float GridSize = 1f;
    private const float MaxDistance = 5f;
    private const int BuildMask = 1 << 0;
    private const ItemType MinecraftItemType = ItemType.KeycardJanitor; // 블록 설치용 아이템
    public readonly Dictionary<BlockType, BlockInfo> Blocks = new();
    private readonly Dictionary<Vector3Int, Block> _world = new();
    public readonly Dictionary<ushort, BlockData> BlockSerials = new();
    private const float MiningTick = 0.1f;      // 진행도를 갱신하는 간격(초)
    private const float SwingInterval = 0.2f;   // 파는 동안 휘두르는 간격(초)
    private const float BreakCooldown = 0.25f;  // 블록 하나를 부순 뒤 다음 블록을 파기 전 대기
    private MinecraftEventHandler _eventHandler;
    private CoroutineHandle _startHandle;

    // 참고: 여기 있는 대부분의 주석들은 클롣드가 해준건데 귀찮아서 안 지움.
    public override void OnEnabled()
    {
        Instance = this;
        _eventHandler = new MinecraftEventHandler(this);
        Blocks.Clear();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var blockAttribute = type.GetCustomAttribute<BlockAttribute>();
            if (blockAttribute == null) continue;

            if (!typeof(Block).IsAssignableFrom(type)) continue;

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
                DestroyTime = blockAttribute.DestroyTime,
                Color = blockAttribute.Color
            });
        }

        _startHandle = Timing.RunCoroutine(OnModeStarted());
        _eventHandler.RegisterEvents();
        MinecraftSetting.Register();
    }

    public override void OnDisabled()
    {
        MinecraftSetting.Unregister();                 // 추가: 키 입력을 먼저 끊음_eventHandler.UnRegisterEvents(); 
        Timing.KillCoroutines(_startHandle);

        foreach (var p in _mining.Keys.ToList())       // 추가: 파는 중이던 코루틴 정리
            StopMining(p);

        foreach (var block in _world.Values)if (block.Schematic != null)
                block.Schematic.Destroy();
        _world.Clear();

        foreach (var player in Player.List)
            player.Items.Where(i => BlockSerials.ContainsKey(i.Serial)).ToList().ForEach(x => player.RemoveItem(x));

        // Dropped 스키매틱도 같이 사라짐
        Pickup.List.Where(p => BlockSerials.ContainsKey(p.Serial)).ToList().ForEach(x => x.Destroy());


        BlockSerials.Clear();
        Blocks.Clear();
        Instance = null;                               // 추가
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
        BlockType randomBlockType = Tools.EnumToList<BlockType>().Where(t => t != BlockType.Air).GetRandomValue();
        AddBlock(randomBlockType, 32, player);

        player.EnableEffect(EffectType.Lightweight, 30);
        if (player.Role == RoleTypeId.Scp096) player.EnableEffect(EffectType.Lightweight, 40);
    }

    private void AddBlock(BlockType type, ushort count, Player player)
    {
        Item item = player.AddItem(MinecraftItemType);
        if (item == null)
            return; // 인벤토리가 가득 찼거나 지급 실패

        BlockSerials[item.Serial] = new BlockData { Type = type, Count = count };
    }

    // Placement / Interaction API
    private bool TryPlaceBlock(Player player, BlockType type)
    {
        if (!Blocks.TryGetValue(type, out var info))
            return false;

        Transform cam = player.CameraTransform;
        if (!Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, MaxDistance, BuildMask)) return false;

        Vector3Int grid = ToGrid(hit.point + hit.normal * (GridSize * 0.05f));
        if (_world.ContainsKey(grid)) return false;

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

        _world[grid] = block;
        block.OnPlaced(player, hit);
        _eventHandler.SwingHeld(player);
        return true;
    }

    public bool TryPlaceHeldBlock(Player player)
    {
        if (player.CurrentItem is not { } item || !BlockSerials.TryGetValue(item.Serial, out BlockData data))
            return false;

        if (!TryPlaceBlock(player, data.Type)) return false;

        if (--data.Count <= 0)
        {
            BlockSerials.Remove(item.Serial);
            player.RemoveItem(item); // 개수가 0이면 아이템 제거
        }
        return true;
    }

    public bool TryInteract(Player player)
    {
        if (!TryGetLookedBlock(player, out Block block)) return false;

        if (!block.OnInteract(player)) return false;

        _eventHandler.SwingHeld(player);
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
        return _world.TryGetValue(grid, out block);
    }

    private static Vector3Int ToGrid(Vector3 pos) => new(
        Mathf.RoundToInt(pos.x / GridSize),
        Mathf.RoundToInt(pos.y / GridSize),
        Mathf.RoundToInt(pos.z / GridSize));

    private static Vector3 ToWorld(Vector3Int grid) => new(
        grid.x * GridSize,
        grid.y * GridSize,
        grid.z * GridSize);

    private sealed class MiningState
    {
        public Block Target;
        public float Progress;
        public float SwingTimer;
        public CoroutineHandle Handle;
    }

    private readonly Dictionary<Player, MiningState> _mining = new();

    public void StartMining(Player player)
    {
        StopMining(player); // 이미 파던 게 있으면 정리 (중복 방지)

        var state = new MiningState();
        _mining[player] = state;
        state.Handle = Timing.RunCoroutine(MiningLoop(player, state));
    }

    public void StopMining(Player player)
    {
        if (!_mining.TryGetValue(player, out MiningState state))
            return;

        Timing.KillCoroutines(state.Handle);
        _mining.Remove(player);
    }

    private IEnumerator<float> MiningLoop(Player player, MiningState state)
    {
        while (player.IsConnected && player.IsAlive)
        {
            if (!TryGetLookedBlock(player, out Block block))
            {
                // 허공이나 블록이 아닌 곳을 보면 리셋
                state.Target = null;
                state.Progress = 0f;
            }
            else
            {
                if (block != state.Target) // 다른 블록을 쳐다보면 리셋
                {
                    state.Target = block;
                    state.Progress = 0f;
                }

                float destroyTime = Mathf.Max(block.Info.DestroyTime, 0.05f);
                state.Progress += MiningTick * GetMiningSpeed(player, block) / destroyTime;

                state.SwingTimer -= MiningTick;
                if (state.SwingTimer <= 0f)
                {
                    _eventHandler.SwingHeld(player);
                    state.SwingTimer = SwingInterval;
                }

                if (state.Progress >= 1f)
                {
                    DestroyBlock(player, block);
                    state.Target = null;
                    state.Progress = 0f;

                    yield return Timing.WaitForSeconds(BreakCooldown);
                    continue;
                }

                player.AddHint("블록 파괴", $"<size=24>{ProgressBar(state.Progress)}</size>", MiningTick * 3);
            }

            yield return Timing.WaitForSeconds(MiningTick);
        }

        // 죽거나 나가서 루프가 끝난 경우 정리
        if (_mining.TryGetValue(player, out MiningState current) && current == state)
            _mining.Remove(player);
    }

    // 도구에 따른 파괴 속도 배율. 지금은 항상 1배이고, 나중에 든 도구에 따라 바꾸는 자리
    private static float GetMiningSpeed(Player player, Block block) => 1f;

    private static string ProgressBar(float progress, int length = 10)
    {
        int filled = Mathf.Clamp(Mathf.RoundToInt(progress * length), 0, length);
        return new string('█', filled) + new string('░', length - filled);
    }

    private void DestroyBlock(Player player, Block block)
    {
        block.OnBroken(player);

        Vector3 center = ToWorld(block.GridPos);

        if (block.Schematic != null)
            block.Schematic.Destroy();

        BlockEffects.SpawnDestroyEffect(block.Info, center);

        try
        {
            Pickup pickup = Pickup.CreateAndSpawn(MinecraftItemType, center);
            if (pickup != null)
            {
                BlockSerials[pickup.Serial] = new BlockData { Type = block.Info.BlockType, Count = 1 };
                var visual = new BlockVisual(block.Info, BlockForm.Dropped, pickup.Transform.position);
                visual.AttachTo(pickup.Transform);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed to spawn dropped block pickup: {e}");
        }

        _world.Remove(block.GridPos);
    }

    // 즉시 파괴 (관리자/테스트용). 파괴 로직은 DestroyBlock을 같이 씀
    public bool TryDestroyBlock(Player player)
    {
        if (!TryGetLookedBlock(player, out Block block))
            return false;

        DestroyBlock(player, block);
        _eventHandler.SwingHeld(player);
        return true;
    }
}