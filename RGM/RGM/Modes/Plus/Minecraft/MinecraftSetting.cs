using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace RGM.Modes;

public static class MinecraftSetting
{
    public const int PlaceKeyId = 9101;
    public const int BreakKeyId = 9102;
    public const int InteractKeyId = 9103;

    public static void Register()
    {
        ServerSpecificSettingsSync.DefinedSettings = new ServerSpecificSettingBase[]
        {
            new SSGroupHeader("마인크래프트"),
            new SSKeybindSetting(PlaceKeyId, "블록 설치", suggestedKey: KeyCode.Z, hint: "들고 있는 블록을 바라보는 곳에 설치합니다."),
            new SSKeybindSetting(BreakKeyId, "블록 파괴", suggestedKey: KeyCode.X, hint: "바라보는 블록을 파괴합니다."),
            new SSKeybindSetting(InteractKeyId, "상호작용", suggestedKey: KeyCode.C, hint: "바라보는 블록과 상호작용합니다."),
        };

        ServerSpecificSettingsSync.SendToAll();

        ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSSInput;
        Exiled.Events.Handlers.Player.Verified += OnVerified;
    }

    public static void Unregister()
    {
        ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSSInput;
        Exiled.Events.Handlers.Player.Verified -= OnVerified;

        ServerSpecificSettingsSync.DefinedSettings = Array.Empty<ServerSpecificSettingBase>();
        ServerSpecificSettingsSync.SendToAll();
    }

    private static void OnVerified(VerifiedEventArgs ev)
    {
        ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
    }

    private static void OnSSInput(ReferenceHub hub, ServerSpecificSettingBase setting)
    {
        if (setting is not SSKeybindSetting { SyncIsPressed: true } keybind)
            return;

        Player player = Player.Get(hub);
        if (player == null || !player.IsAlive)
            return;

        Minecraft minecraft = Minecraft.Instance;

        switch (keybind.SettingId)
        {
            case PlaceKeyId:
                minecraft.TryPlaceHeldBlock(player);
                break;
            case BreakKeyId:
                minecraft.TryDestroyBlock(player);
                break;
            case InteractKeyId:
                minecraft.TryInteract(player);
                break;
        }
    }
}