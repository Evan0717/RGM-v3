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
        if (setting is not SSKeybindSetting keybind)
            return;

        Player player = Player.Get(hub);
        Minecraft minecraft = Minecraft.Instance;
        if (player == null || minecraft == null)
            return;

        bool pressed = keybind.SyncIsPressed;

        switch (keybind.SettingId)
        {
            case BreakKeyId:
                // 누르면 시작, 떼면 중단 (죽어 있으면 시작하지 않음)
                if (pressed && player.IsAlive)
                    minecraft.StartMining(player);
                else
                    minecraft.StopMining(player);
                break;

            case PlaceKeyId when pressed && player.IsAlive:
                minecraft.TryPlaceHeldBlock(player);
                break;

            case InteractKeyId when pressed && player.IsAlive:
                minecraft.TryInteract(player);
                break;
        }
    }
}