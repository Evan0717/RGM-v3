using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.MicroHID.Modules;
using MEC;
using RGM.API.Features;

namespace RGM.Modes;

public static class PlayerFeatures
{
    private static CoroutineHandle _hidCoroutine;

    public static void Activate()
    {
        Exiled.Events.Handlers.Player.ChangingMicroHIDState += OnChanging;
        Exiled.Events.Handlers.Player.Spawned += OnSpawn;
        Exiled.Events.Handlers.Player.Died += OnDied;
        Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
        Exiled.Events.Handlers.Player.ThrowingRequest += OnThrowingRequest;

    }

    public static void DeActivate()
    {
        Exiled.Events.Handlers.Player.ChangingMicroHIDState -= OnChanging;
        Exiled.Events.Handlers.Player.Spawned -= OnSpawn;
        Exiled.Events.Handlers.Player.Died -= OnDied;
        Exiled.Events.Handlers.Player.SearchingPickup -= OnSearchingPickup;
        Exiled.Events.Handlers.Player.ThrowingRequest -= OnThrowingRequest;
        
        UnloadEffects();
    }

    internal static void AddEffects()
    {
        try
        {
            foreach (var player in PlayerManager.List.Where(player =>
                         !player.IsNonePlayer() && !player.IsDead && !player.IsNPC))
            {
                UnloadEffects();
                player.AddEffect(EffectType.MovementBoost, (byte)(SpeedStore.Count * 2));
                player.AddEffect(EffectType.Scp1853, Math.Min(SpeedStore.Count, (byte)5));
                // 곧 적용예정.
                // player.AddEffect(EffectType.Scp1853, Math.Min(SpeedStore.Count, (byte)100));
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error while adding effects, Deception: {e.Message}");
        }
    }

    internal static void UnloadEffects()
    {
        try
        {
            foreach (var player in PlayerManager.List.Where(player =>
                         player != null && !player.IsDead && !player.IsNonePlayer() && !player.IsNPC))
            {
                player.RemoveEffect(EffectType.MovementBoost, 255);
                player.RemoveEffect(EffectType.Scp1853, 5);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error while removing effects, Deception: {e.Message}");
        }
    }

    private static void OnChanging(ChangingMicroHIDStateEventArgs ev)
    {
        SpeedStore.RailgunCooldown = SpeedStore.Count > 30 ? 
            SpeedStore.RailgunCooldown + SpeedStore.Count * 0.01f :
            SpeedStore.RailgunCooldown;
        if (!Timing.IsRunning(_hidCoroutine))
            _hidCoroutine = Timing.RunCoroutine(Run());
        return;

        IEnumerator<float> Run()
        {
            while (SpeedStore.IsEnabled)
            {
                foreach (var items in Item.List.Where(x =>
                             x.Type == ItemType.MicroHID))
                {
                    if (items is not MicroHid hid) continue;
                    if (hid.State is not MicroHidPhase.WindingUp)
                        continue;

                    hid.WindUpProgress += 0.1f;
                    
                }
                foreach (var items in Item.List.Where(x =>
                             x.Type == ItemType.MicroHID))
                {
                    if (items is not MicroHid hid) continue;
                    if (hid.State is not MicroHidPhase.WindingDown)
                        continue;

                    hid.WindUpProgress -= 0.1f;
                    
                }
                yield return Timing.WaitForSeconds(SpeedStore.RailgunCooldown - SpeedStore.Count * 0.1f);
            }
        }
    }
    
    private static void OnSearchingPickup(SearchingPickupEventArgs ev)
    {
        ev.SearchTime -= SpeedStore.Count * 0.1f;
    }

    private static void OnThrowingRequest(ThrowingRequestEventArgs ev)
    {
        ev.Throwable.PinPullTime -= SpeedStore.Count * 0.1f;
    }
    
    private static void OnDied(DiedEventArgs ev)
    {
        if (!(SpeedStore.Count > 125))
            SpeedStore.Count++;

        AddEffects();
    }

    private static void OnSpawn(SpawnedEventArgs ev)
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (ev.Player == null || !ev.Player.IsAlive || ev.Player.IsNonePlayer()) return;
            AddEffects();
        });
        
    }
}