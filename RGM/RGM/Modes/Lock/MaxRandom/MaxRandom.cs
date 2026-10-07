using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using RGM.API.Features;
using UnityEngine;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Scp173;
using PlayerRoles;
using PlayerRoles.PlayableScps.HumeShield;
using Random = UnityEngine.Random;
using InventorySystem.Items;
using Exiled.API.Features.Doors;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Server;
using Respawning;
using RGM.Patches;

namespace RGM.Modes;

[Mode(ModeCategory.Public, ModeInfo.Lock, ModeType.MaxRandom)]
public class MaxRandom : Mode
{
    public override string Name => "MAX RANDOM";
    public override string Description => "모든 것을 랜덤으로 정합니다!";
    public override string Detail =>
        """
        모든 시스템이 랜덤으로 결정됩니다.
        
        스폰 진영, 위치, 아이템, 사이즈, 지원, 최대 체력, 데미지, 치유량, 효과, 자동핵 시간
        모두 랜덤으로 정해집니다.
        """;
    public override string Color => "FA67FC";
    public override string Author => "DeniA";

    private readonly List<EffectType> _ignoredEffect =
    [
        EffectType.PocketCorroding,
        EffectType.PitDeath,
        EffectType.CardiacArrest,
        EffectType.Poisoned,
        EffectType.SpawnProtected,
        EffectType.Ensnared,
        EffectType.Flashed,
        EffectType.SeveredHands,
        EffectType.Invisible,
        EffectType.None,
        EffectType.MovementBoost,
        EffectType.Slowness,
        EffectType.HeavyFooted,
        EffectType.Lightweight,
    ];
    
    private static readonly List<RoleTypeId> IgnoredRoles =
    [
        RoleTypeId.Flamingo,
        RoleTypeId.AlphaFlamingo,
        RoleTypeId.ChaosFlamingo,
        RoleTypeId.NtfFlamingo,
        RoleTypeId.ZombieFlamingo,
        RoleTypeId.Spectator,
        RoleTypeId.Destroyed,
        RoleTypeId.Overwatch,
        RoleTypeId.Filmmaker,
        RoleTypeId.None,
        RoleTypeId.CustomRole,
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
    private CoroutineHandle _randomBox;
    private CoroutineHandle _randomFactionSpawn;
    private readonly AutoWarhead _autoWarhead = new(Random.Range(8, 21), 1);
    
    public override void OnEnabled()
    {
        _isEnabled = true;
        
        Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        Exiled.Events.Handlers.Player.Hurting += OnHurting;
        Exiled.Events.Handlers.Player.Healing += OnHealing;
        
        Exiled.Events.Handlers.Scp079.GainingExperience += OnGainingExperience;
        Exiled.Events.Handlers.Scp079.InteractingTesla += OnInteractingTesla;
        Exiled.Events.Handlers.Scp079.RoomBlackout += OnRoomBlackout;
        Exiled.Events.Handlers.Scp079.ZoneBlackout += OnZoneBlackout;
        Exiled.Events.Handlers.Scp079.LockingDown += OnLockingDown;
        Exiled.Events.Handlers.Scp079.LosingSignal += OnLosingSignal;
        
        Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;

        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        
        _randomBox = Timing.RunCoroutine(RandomBoxCoroutine());
        _randomFactionSpawn = Timing.RunCoroutine(RandomFactionSpawnCoroutine());
        _autoWarhead.RunCoroutine();
    }
    public override void OnDisabled()
    {
        _isEnabled = false;
        
        Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
        Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        Exiled.Events.Handlers.Player.Healing -= OnHealing;
        
        Exiled.Events.Handlers.Scp079.GainingExperience -= OnGainingExperience;
        Exiled.Events.Handlers.Scp079.InteractingTesla -= OnInteractingTesla;
        Exiled.Events.Handlers.Scp079.RoomBlackout -= OnRoomBlackout;
        Exiled.Events.Handlers.Scp079.ZoneBlackout -= OnZoneBlackout;
        Exiled.Events.Handlers.Scp079.LockingDown -= OnLockingDown;
        Exiled.Events.Handlers.Scp079.LosingSignal -= OnLosingSignal;
        
        Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;
        
        Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

        Timing.KillCoroutines(_randomBox);
        Timing.KillCoroutines(_randomFactionSpawn);
        _autoWarhead.KillCoroutine();
        foreach (var player in PlayerManager.List)
            Timing.KillCoroutines(GetSpawnInventoryCoroutineName(player));
    }
    private void OnSpawned(SpawnedEventArgs ev)
    {
        if (ev.Player.IsNonePlayer())
            return;
        
        if (!_isEnabled || !ev.Player.IsAlive)
            return;

        // 역할 변경 전의 스폰에서는 아이템을 지급하지 않습니다.
        // Role.Set이 발생시키는 ItemUsage 스폰에서만 최종 역할의 인벤토리를 구성합니다.
        if (ev.Reason == SpawnReason.ItemUsage)
        {
            Spawned(ev.Player);
            StartSpawnCoroutine(ev.Player);
            return;
        }

        RoleTypeId roleType = SelectRole(ev.Player);
        ev.Player.Role.Set(roleType, SpawnReason.ItemUsage, RoleSpawnFlags.AssignInventory);
    }

    private static string GetSpawnInventoryCoroutineName(Player player) =>
        $"MaxRandomSpawnInventory_{player.UserId}";

    private static void StartSpawnCoroutine(Player player)
    {
        var coroutineName = GetSpawnInventoryCoroutineName(player);
        Timing.KillCoroutines(coroutineName);
        Timing.RunCoroutine(SpawnCoroutine(player), coroutineName);
    }

    private void Spawned(Player player)
    {
        Timing.CallDelayed(0.05f, () =>
        {
            player.MaxHealth *= Random.Range(0.5f, 2.5f);
            player.Health = player.MaxHealth;

            if (!player.IsScpRole()) return;

            if (player.ReferenceHub.roleManager.CurrentRole is not IHumeShieldedRole { HumeShieldModule: { } humeShieldModule })
                return;

            player.MaxHumeShield = Mathf.Max(100f, humeShieldModule.HsMax * Random.Range(0.5f, 2.5f));
            player.HumeShield = player.MaxHumeShield;
        });

        Timing.CallDelayed(0.05f, () =>
        {
            var movementSpeedIntensity = Random.Range(-50, 101);
            switch (movementSpeedIntensity)
            {
                case 0:
                    break;
                case > 0:
                    player.AddEffect(EffectType.MovementBoost, movementSpeedIntensity);
                    break;
                case < 0:
                    player.AddEffect(EffectType.Slowness, -movementSpeedIntensity);
                    break;
            }
            var jumpIntensity = Random.Range(-50, 101);
            switch (jumpIntensity)
            {
                case 0:
                    break;
                case > 0:
                    player.AddEffect(EffectType.Lightweight, jumpIntensity);
                    break;
                case < 0:
                    player.AddEffect(EffectType.HeavyFooted, -jumpIntensity);
                    break;
            }
            
            List<EffectType> effects = [.. Tools.EnumToList<EffectType>().Where(x => !_ignoredEffect.Contains(x))];

            var effect = effects.GetRandomValue();
            var intensity = Convert.ToByte(Random.Range(1, 128));

            player.EnableEffect(effect, intensity);
            player.AddHint("랜덤효과 안내", $"<color=#D0FA58>{effect}</color> 효과가 {intensity}만큼 적용되는 중입니다.", short.MaxValue);

        });
        
        Timing.CallDelayed(0.1f, () => 
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
    
    private static void OnHealing(HealingEventArgs ev)
    {
        if (ev.Player == null) return;
        ev.Amount *= Random.Range(0.1f, 5.1f);
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

    private static void OnBlinking(BlinkingEventArgs ev)
    {
        if (_isEnabled)
            ev.BlinkCooldown = Random.Range(0.2f, 5.0f);
    }

    private static void OnGainingExperience(GainingExperienceEventArgs ev)
    {
        if (_isEnabled)
            ev.Amount = Random.Range(1, 101);
    }

    private static void OnInteractingTesla(InteractingTeslaEventArgs ev)
    {
        if (_isEnabled)
            ev.Scp079.TeslaAbility._cooldown = Random.Range(1, 31);
    }

    private static void OnRoomBlackout(RoomBlackoutEventArgs ev)
    {
        if (_isEnabled)
            ev.Cooldown = Random.Range(1, 61);
    }

    private static void OnZoneBlackout(ZoneBlackoutEventArgs ev)
    {
        if (!_isEnabled) return;
        try
        {
            ev.Cooldown = Random.Range(5, 121);
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    private static void OnLockingDown(LockingDownEventArgs ev)
    {
        if (_isEnabled)
            ev.Scp079.LockdownRoomAbility._cooldown = Random.Range(1, 31);
    }

    private static void OnLosingSignal(LosingSignalEventArgs ev)
    {
        if (_isEnabled)
            ev.Scp079.Scp2176LostTime = Random.Range(1, 31);
    }
    
    private static IEnumerator<float> SpawnCoroutine(Player player)
    {
        List<ItemType> itemList = [.. Tools.EnumToList<ItemType>().Where(x => !x.IsAmmo())];
        
        yield return Timing.WaitForOneFrame;
        
        if (!player.IsAlive || player.Role.Type == RoleTypeId.Scp079)
            yield break;
        
        yield return Timing.WaitForSeconds(0.1f);
        player.ClearInventory();

        // 탄약 아이템은 인벤토리 슬롯을 점유하므로, 예비 탄약으로 직접 지급합니다.
        // 아래 랜덤 아이템 6개가 슬롯 부족으로 누락되지 않도록 합니다.
        player.AddAmmo(AmmoType.Nato9, Convert.ToByte(Random.Range(1, 256)));
        player.AddAmmo(AmmoType.Nato556, Convert.ToByte(Random.Range(1, 256)));
        player.AddAmmo(AmmoType.Nato762, Convert.ToByte(Random.Range(1, 256)));
        player.AddAmmo(AmmoType.Ammo12Gauge, Convert.ToByte(Random.Range(1, 64)));
        player.AddAmmo(AmmoType.Ammo44Cal, Convert.ToByte(Random.Range(1, 32)));

        for (int i = 1; i < 7; i++)
        {
            var item = itemList.GetRandomValue();
                
            player.AddItem(item);

            yield return Timing.WaitForSeconds(1);
        }
    }

    private static IEnumerator<float> RandomBoxCoroutine()
    {
        yield return Timing.WaitForSeconds(Convert.ToByte(Random.Range(10, 61)));

        foreach (var player in PlayerManager.List.Where(x => x.IsAlive && x.Role.Type != RoleTypeId.Scp079))
            try
            {
                var itemList = Tools.EnumToList<ItemType>();
                var item = itemList.GetRandomValue();
                player.AddItem(item);

                player.AddHint("랜덤박스", $"<color=#F3F781>{item.GetName()}</color>(을)를 지급받았습니다.",
                    5);
            }
            catch (KeyNotFoundException e)
            { Log.Warn($"[RGM] RandomItem card fetch failure: {e.Message}"); }
            catch (Exception ex) { Log.Error($"[RGM] RandomItem Mode Error: {ex}"); }
    }

    private static IEnumerator<float> RandomFactionSpawnCoroutine()
    {
        while (true)
        {
            var faction = Convert.ToByte(Random.Range(1, 101)) <= 50 ? Faction.FoundationEnemy : Faction.FoundationStaff;
            Timing.CallDelayed(Convert.ToInt16(Random.Range(60, 301)), () => 
            {
                Respawn.GrantTokens(faction, 1);
                if (WaveManager.TryGet(faction, out var wave)) WaveManager.Spawn(wave);
            });
            
            yield return Timing.WaitForOneFrame;
        }
    }
    
    private static void OnRoundEnded(RoundEndedEventArgs ev)
    {
        List<Player> players = [.. PlayerManager.List.Where(x => !x.IsNPC)];

        if (players.Count == 0)
            return;

        // 1 ~ 실제 대상 후보 수 (int 버전 Range의 상한은 제외됨)
        var targetCount = Random.Range(1, players.Count + 1);

        // Fisher–Yates shuffle: 각 플레이어는 정확히 한 번만 존재
        for (int i = players.Count - 1; i > 0; i--)
        {
            var randomIndex = Random.Range(0, i + 1);
            (players[i], players[randomIndex]) = (players[randomIndex], players[i]);
        }

        List<Player> winners = [.. players.Take(targetCount)];
        
        var reward = Random.Range(1, players.Count + 1);

        Timing.RunCoroutine(Tools.SetWinner(winners, reward));
    }
}