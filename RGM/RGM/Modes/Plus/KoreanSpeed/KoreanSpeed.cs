using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using RGM.API.Features;

namespace RGM.Modes;

[Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.KoreanSpeed)]
public class KoreanSpeed : Mode
{
    public override string Name => "한국인이 좋아하는 속도";
    public override string Description => "누군가가 사망할 때마다 모두의 속도가 증가합니다.";

    public override string Detail =>
        "<b><color=#FB00FF>슈</color><color=#D200D5>우</color><color=#A901AB>우</color><color=#800282>우</color><color=#570358>웅</color><color=#2E042E>화</color></b>";

    public override string Color => "5882FA";

    public static KoreanSpeed Instance;

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Spawned -= OnSpawn;
        Exiled.Events.Handlers.Player.Died -= OnDied;
        Exiled.Events.Handlers.Player.SearchingPickup -= OnSearchingPickup;
        Exiled.Events.Handlers.Player.ThrowingRequest -= OnThrowingRequest;
        SpeedStore.Disable();
        UnloadEffects();
    }

    public override void OnEnabled()
    {
        Exiled.Events.Handlers.Player.Spawned += OnSpawn;
        Exiled.Events.Handlers.Player.Died += OnDied;
        Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
        Exiled.Events.Handlers.Player.ThrowingRequest += OnThrowingRequest;
        SpeedStore.Clear();
        SpeedStore.Ignition();
    }

    private static void OnDied(DiedEventArgs ev)
    {
        if (SpeedStore.Count != 125)
            SpeedStore.Count++;

        AddEffects();
    }

    private static void OnSearchingPickup(SearchingPickupEventArgs ev)
    {
        ev.SearchTime -= SpeedStore.Count * 0.1f;
    }

    private static void OnThrowingRequest(ThrowingRequestEventArgs ev)
    {
        ev.Throwable.PinPullTime -= SpeedStore.Count * 0.1f;
    }

    private static void OnSpawn(SpawnedEventArgs ev)
    {
        Timing.CallDelayed(Timing.WaitForOneFrame, () =>
        {
            if (ev.Player == null || !ev.Player.IsAlive || ev.Player.IsNonePlayer()) return;
            AddEffects();
        });
    }

    internal static void AddEffects()
    {
        try
        {
            foreach (var player in PlayerManager.List.Where(player => player != null && !player.IsDead))
            {
                player.EnableEffect(EffectType.MovementBoost, (byte)(SpeedStore.Count * 2));
                player.EnableEffect(EffectType.Scp1853, SpeedStore.Count <= 5 ? SpeedStore.Count : (byte)5);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error while adding effects, Deception: {e.Message}");
        }
    }

    private static void UnloadEffects()
    {
        try
        {
            foreach (var player in PlayerManager.List.Where(player => player != null && !player.IsDead))
            {
                player.DisableEffect(EffectType.MovementBoost);
                player.DisableEffect(EffectType.Scp1853);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error while removing effects, Deception: {e.Message}");
        }
    }
}