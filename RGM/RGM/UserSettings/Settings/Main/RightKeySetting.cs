using SecretAPI.Features.UserSettings;
using UnityEngine;

namespace RGM.UserSettings;

public static partial class MainSettingManager
{
    private sealed partial class RightKeySetting()
        : CustomKeybindSetting(12058, "오른쪽 이동키ㅣRight movement key", KeyCode.RightArrow) 
    {
        public override CustomHeader Header => Setting;
        protected override CustomSetting CreateDuplicate() => new RightKeySetting();

        protected override void HandleSettingUpdate()
        {
        }
    }
}