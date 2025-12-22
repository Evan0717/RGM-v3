using AdminToys;
using CommandSystem.Commands.RemoteAdmin;
using CustomPlayerEffects;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Scp244;
using Exiled.Events.EventArgs.Server;
using Interactables.Interobjects.DoorUtils;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using Respawning;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.Original)]
    public class Original : Mode
    {
        public override string Name => "오리지널";
        public override string Description => "가끔은 도파민이 없는 기본을 해보시는 건 어떠신지..?";
        public override string Detail =>
"""
모드가 존재하지 않습니다.
""";
        public override string Color => "FFFFFF";

        public static Original Instance;

        public override void OnEnabled()
        {
            // 히히 날먹
        }

        public override void OnDisabled()
        {
        }
    }
}
