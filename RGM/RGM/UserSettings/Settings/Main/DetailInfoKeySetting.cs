using SecretAPI.Features.UserSettings;
using UnityEngine;

namespace RGM.UserSettings;

public static partial class MainSettingManager
{
    private sealed partial class DetailInfoKeySetting() : CustomKeybindSetting(12060, "자세한 설명 보기ㅣShow detailed info",
        KeyCode.F1, hint: "현재 모드의 자세한 정보를 확인합니다.") 
    {
        public override CustomHeader Header => Setting;
        
        protected override CustomSetting CreateDuplicate() => new DetailInfoKeySetting();

        protected override void HandleSettingUpdate()
        {
        }
    }
}