using Exiled.API.Features;
using UserSettings.ServerSpecific;
using static RGM.Variables.Variable;

namespace RGM.Modes.Plus.Store
{
    public static class StoreSetting
    {
        public static void OnSSInput(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            if (setting is not SSKeybindSetting keybind || !keybind.SyncIsPressed)
                return;

            Player player = Player.Get(sender);

            if (CurrentMode == ModeType.Store && setting.SettingId == 12060 && Modes.Store.Instance != null)
            {
                return;
            }

            if (CurrentMode == ModeType.Store && Modes.Store.Instance != null)
            {
                if (setting.SettingId == 12055)
                {
                    Modes.Store.Instance.MoveSelectionCursor(player, -1);
                    return;
                }

                if (setting.SettingId == 12056)
                {
                    Modes.Store.Instance.MoveSelectionCursor(player, 1);
                    return;
                }

                if (setting.SettingId == 12059)
                {
                    Modes.Store.Instance.ConfirmSelectionByCursor(player, out _);
                    return;
                }
            }
        }
    }
}
