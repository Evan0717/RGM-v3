using SecretAPI.Features.UserSettings;
using UnityEngine;

namespace RGM.UserSettings;

public static partial class MainSettingManager
{
    private sealed partial class UpKeySetting() : CustomKeybindSetting(12055, "위 이동키ㅣUp movement key", KeyCode.UpArrow) 
    {
        public override CustomHeader Header => Setting;
        protected override CustomSetting CreateDuplicate() => new UpKeySetting();

        protected override void HandleSettingUpdate()
        {
        }
    }
}