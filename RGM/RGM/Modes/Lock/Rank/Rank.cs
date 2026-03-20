using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CustomRendering;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using HarmonyLib;
using MEC;
using Mirror;
using MultiBroadcast;
using UnityEngine;
using Exiled.API.Enums;
using PlayerRoles;

using RGM.API.Features;
using ProjectMER.Features;

using static RGM.Variables.Variable;
using Respawning;
using Exiled.Events.EventArgs.Player;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Lock, ModeType.Rank)]
    class Rank : Mode
    {
        public override string Name => "경쟁전";
        public override string Description => "직업별로 변칙성, 가젯, 기어를 설정하세요.";
        public override string Detail =>
"""
모든 능력들은 스폰 후, 30초 뒤에 적용됩니다.

[ESC] -> [Settings] -> [Server-specific]
""";
        public override string Color => "ea524c";

        public override void OnEnabled()
        {
        }

        public override void OnDisabled()
        {
        }
    }
}
