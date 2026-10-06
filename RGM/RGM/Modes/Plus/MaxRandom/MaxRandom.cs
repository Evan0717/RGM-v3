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

namespace RGM.Modes;

[Mode(ModeCategory.Private, ModeInfo.Plus, ModeType.MaxRandom)]
public class MaxRandom  : Mode
{
    public override string Name => "MAX RANDOM";
    public override string Description => "모든 것을 랜덤으로 정합니다!";
    public override string Detail =>
        """
        모든 시스템이 랜덤으로 변경됩니다.
        
        스폰 진영, 스폰 위치, 시작 아이템, 사이즈, 지원 진영, 최대 체력, 데미지 모두 랜덤으로 정해집니다.
        """;
    public override string Color => "BFFF00";
    public override string Author => "DeniA";

    private static readonly List<RoleTypeId> IgnoredRoles =
    [
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
        // 모든 인원 진영을 랜덤으로 섞기
        { RoleTypeId.ClassD, 
            [.. Tools.EnumToList<RoleTypeId>().Where(x => !IgnoredRoles.Contains(x))] },
        { RoleTypeId.Scientist, 
            [.. Tools.EnumToList<RoleTypeId>().Where(x => !IgnoredRoles.Contains(x))] },
        { RoleTypeId.FacilityGuard,
            [.. Tools.EnumToList<RoleTypeId>().Where(x => !IgnoredRoles.Contains(x))]
        }
    };
    
    private static bool _isEnabled;
    private CoroutineHandle _onModeStarted;
    
    public override void OnEnabled()
    {
        _isEnabled = true;
        
        Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;

        _onModeStarted = Timing.RunCoroutine(OnModeStarted());
    }
    public override void OnDisabled()
    {
        _isEnabled = false;
        
        Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;

        Timing.KillCoroutines(_onModeStarted);
    }
    private IEnumerator<float> OnModeStarted()
    {
        foreach (var player in PlayerManager.List)
        {
            RoleTypeId roleType = SelectRole(player);
            player.Role.Set(roleType, SpawnReason.ItemUsage, RoleSpawnFlags.AssignInventory);
            Spawned(player);
            Timing.RunCoroutine(SpawnCoroutine(player));
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
        Timing.CallDelayed(0.5f, () =>
        {
            player.MaxHealth *= Random.Range(0.5f, 2.5f);
            player.Health = player.MaxHealth;
            
            if (!player.IsScpRole()) return;
            
            player.MaxHumeShield *= Random.Range(0.5f, 2.5f);
            player.HumeShield = player.MaxHumeShield;
        });
        
        Timing.CallDelayed(0.5f, () => 
        {
            player.Scale = new Vector3(Random.Range(0.1f, 1.2f), Random.Range(0.3f, 1.2f), Random.Range(0.1f, 1.2f));
        });
        
        if (Warhead.IsDetonated) return;

        var selectedDoor = Exiled.API.Features.Map.IsLczDecontaminated
            ? Door.List.Where(x => !x.IsElevator && x.Zone != ZoneType.LightContainment && !x.Type.ToString().Contains("Scp079")).ToList().GetRandomValue()
            : Door.List.Where(x => !x.IsElevator && !x.Type.ToString().Contains("Scp079")).ToList().GetRandomValue();

        player.Position = new Vector3(selectedDoor.Position.x, selectedDoor.Position.y + 2, selectedDoor.Position.z);
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker == null ||
            !HitboxIdentity.IsEnemy(ev.Attacker.ReferenceHub, ev.Player.ReferenceHub) ||
            ApplyFixedDamage.IsApplying) return;

        ev.IsAllowed = false;
        if (ApplyFixedDamage.Apply(ev.Attacker, ev.Player, Random.Range(1, sbyte.MaxValue)))
            ev.Attacker.ShowHitMarker();
    }
    
    private RoleTypeId SelectRole(Player player)
    {
        if (_roleTypeIds.TryGetValue(player.Role.Type, out var roles))
        {
            return roles.GetRandomValue();
        }

        if (player.IsScpRole() || player.IsNTF || player.IsCHI)
            return Tools.EnumToList<RoleTypeId>().
                Where(x => !IgnoredRoles.Contains(x)).GetRandomValue();

        return RoleTypeId.Tutorial;
    }
    private static IEnumerator<float> SpawnCoroutine(Player player)
    {
        if (!player.IsAlive)
            yield break;
        
        yield return Timing.WaitForSeconds(0.5f);
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
        yield return Timing.WaitForSeconds(20f);

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