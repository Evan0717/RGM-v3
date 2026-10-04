using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;
using UnityEngine;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using Random = UnityEngine.Random;
using InventorySystem.Items;
using Exiled.API.Features.Doors;
using Exiled.API.Enums;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using Exiled.API.Enums;
using RGM.API.Features;
using PlayerRoles;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

namespace RGM.Modes;

[Mode(ModeCategory.Private, ModeInfo.Plus, ModeType.MaxRandom)]
public class MaxRandom  : Mode
{
    public override string Name => "MAX RANDOM";
    public override string Description => "모든 것을 랜덤으로 정합니다!";
    public override string Detail =>
        """
        모든 시스템이 랜덤으로 변경됩니다.
        
        스폰 진영, 스폰 위치, 아이템, 사이즈, 지원 진영, 최대 체력이 모두 랜덤으로 정해집니다.
        """;
    public override string Color => "BFFF00";
    public override string Author => "DeniA";

    private static readonly List<RoleTypeId> IgnoredRoles =
    [
        RoleTypeId.Scp3114,
        RoleTypeId.Scp0492,
        RoleTypeId.Flamingo,
        RoleTypeId.AlphaFlamingo,
        RoleTypeId.ChaosFlamingo,
        RoleTypeId.NtfFlamingo,
        RoleTypeId.ZombieFlamingo,
        RoleTypeId.Destroyed,
        RoleTypeId.Overwatch,
        RoleTypeId.Filmmaker,
        RoleTypeId.None
    ];

    private readonly Dictionary<RoleTypeId, List<RoleTypeId>> _roleTypeIds = new()
    {
        { RoleTypeId.ClassD, [RoleTypeId.Scientist] },
        { RoleTypeId.Scientist, [RoleTypeId.ClassD] },
        { RoleTypeId.FacilityGuard,
            [.. Tools.EnumToList<RoleTypeId>().Where(x => x.IsScpRole() && !IgnoredRoles.Contains(x))]
        }
    };
    
    private static bool _isEnabled;
    private CoroutineHandle _onModeStarted;
    
    public override void OnEnabled()
    {
        _isEnabled = true;
        Exiled.Events.Handlers.Player.Spawned += OnSpawned;

        _onModeStarted = Timing.RunCoroutine(OnModeStarted());
    }
    public override void OnDisabled()
    {
        _isEnabled = false;
        Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

        Timing.KillCoroutines(_onModeStarted);
    }
    private IEnumerator<float> OnModeStarted()
    {
        foreach (var player in PlayerManager.List)
        {
            Spawned(player);
            Timing.RunCoroutine(SpawnCoroutine(player));
            RoleTypeId roleType = SelectRole(player);
            player.Role.Set(roleType, SpawnReason.ItemUsage, RoleSpawnFlags.AssignInventory);
            Timing.RunCoroutine(RandomBoxCoroutine());
        }
        yield break;
    }

    private void OnSpawned(SpawnedEventArgs ev)
    {
        if (ev.Player.IsNonePlayer())
            return;
        
        if (_isEnabled)
            Timing.RunCoroutine(SpawnCoroutine(ev.Player));
        
        Spawned(ev.Player);
        if (!ev.Player.IsAlive || ev.Reason == SpawnReason.ItemUsage) return;
        RoleTypeId roleType = SelectRole(ev.Player);
        ev.Player.Role.Set(roleType, SpawnReason.ItemUsage, RoleSpawnFlags.AssignInventory);
    }

    private static void Spawned(Player player)
    {
        Timing.CallDelayed(1f, () => 
        {
            player.Scale = new Vector3(Random.Range(0.2f, 1.2f), Random.Range(0.2f, 1.2f), Random.Range(0.2f, 1.2f));
        });
        
        if (Warhead.IsDetonated) return;

        var selectedDoor = Exiled.API.Features.Map.IsLczDecontaminated
            ? Door.List.Where(x => !x.IsElevator && x.Zone != ZoneType.LightContainment && !x.Type.ToString().Contains("Scp079")).ToList().GetRandomValue()
            : Door.List.Where(x => !x.IsElevator && !x.Type.ToString().Contains("Scp079")).ToList().GetRandomValue();

        player.Position = new Vector3(selectedDoor.Position.x, selectedDoor.Position.y + 2, selectedDoor.Position.z);
    }
    
    private RoleTypeId SelectRole(Player player)
    {
        if (_roleTypeIds.TryGetValue(player.Role.Type, out var roles))
        {
            return roles.GetRandomValue();
        }

        if (player.IsScpRole())
            return RoleTypeId.FacilityGuard;

        if (player.IsNTF)
            return Tools.EnumToList<RoleTypeId>()
                .Where(x => x.IsChaos() && x != RoleTypeId.None).GetRandomValue();

        if (player.IsCHI)
            return Tools.EnumToList<RoleTypeId>()
                .Where(x => x.IsNtf() && x != RoleTypeId.None).GetRandomValue();

        return RoleTypeId.Tutorial;
    }
    private static IEnumerator<float> SpawnCoroutine(Player player)
    {
        if (!player.IsAlive)
            yield break;

        yield return Timing.WaitForOneFrame;

        player.ClearInventory();
        for (int i = 1; i < 7; i++) {
            player.AddItem(ItemType.Ammo9x19);
        }
        for (int i = 1; i < 3; i++)
        {
            player.AddItem(ItemType.Ammo556x45);
            player.AddItem(ItemType.Ammo762x39);
            player.AddItem(ItemType.Ammo12gauge);
            player.AddItem(ItemType.Ammo44cal);
        }
        for (int i = 1; i < 7; i++)
        {
            List<ItemType> itemList = Tools.EnumToList<ItemType>();
            ItemType item = itemList.GetRandomValue();
                
            player.AddItem(item);

            yield return Timing.WaitForSeconds(1);
        }
    }

    private IEnumerator<float> RandomBoxCoroutine()
    {
        yield return Timing.WaitForSeconds(15f);

        foreach (var player in PlayerManager.List.Where(x => x.IsAlive && x.Role.Type != RoleTypeId.Scp079))
            try
            {
                List<ItemType> itemList = Tools.EnumToList<ItemType>();
                ItemType item = itemList.GetRandomValue();
                player.AddItem(item);

                player.AddHint("랜덤박스", $"<color=#F3F781>{item.GetName()}</color>(을)를 지급받았습니다.",
                    5);
            }
            catch (KeyNotFoundException e)
            { Log.Warn($"[RGM] RandomItem card fetch failure: {e.Message}"); }
            catch (Exception ex) { Log.Error($"[RGM] RandomItem Mode Error: {ex}"); }
    }
}