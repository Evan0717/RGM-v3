using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace RGM
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("이것이 참일 시, 출력 시스템은 영어로 동작할 것입니다. (If true, the output will be in English.)")]
        public bool EN { get; set; } = false;
        [Description("여기에 특정 모드의 Enum을 입력하면 그 모드가 고정적으로 나옵니다. 랜덤게임모드 관련 기능들이 일부 차단됩니다.")]
        public List<ModeType> FixedModes { get; set; } = new();
    }
}
