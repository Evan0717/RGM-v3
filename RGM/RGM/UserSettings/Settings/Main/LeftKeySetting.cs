using SecretAPI.Features.UserSettings;
using UnityEngine;

namespace RGM.UserSettings;

public static partial class MainSettingManager
{
    private sealed partial class LeftKeySetting()
        : CustomKeybindSetting(12057, "왼쪽 이동키ㅣLeft movement key", KeyCode.LeftArrow) 
    {
        public override CustomHeader Header => Setting;
        protected override CustomSetting CreateDuplicate() => new LeftKeySetting();

        protected override void HandleSettingUpdate()
        {
        }
    }
}