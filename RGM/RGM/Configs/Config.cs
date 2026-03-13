using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;
using RGM.Modes;

namespace RGM
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public bool EN { get; set; } = false;
        [Description("여기에 특정 모드의 Enum을 입력하면 그 모드가 고정적으로 나옵니다. 랜덤게임모드 관련 기능들이 일부 차단됩니다.")]
        public List<ModeType> FixedModes { get; set; } = new();
    }
}
