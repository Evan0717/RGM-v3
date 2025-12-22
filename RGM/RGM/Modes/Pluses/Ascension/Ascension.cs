using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using RGM.API;
using UnityEngine;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using RGM.API.Features;
using static RGM.Variables.Variable;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Plus, ModeType.Ascension)]
    public class Ascension : Mode
    {
        public override string Name => "승천";
        public override string Description => "점프하면 승천합니다, 너무 멀리요.";
        public override string Detail =>
"""
점프하면 하늘 나라까지 갈 수 있데요!
""";
        public override string Color => "F5DA81";

        public static Ascension Instance;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Jumping += OnJumping;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Jumping -= OnJumping;
        }

        public void OnJumping(JumpingEventArgs ev)
        {
            Timing.RunCoroutine(Tools.DoRocket(ev.Player, ev.Player, 1));
        }
    }
}
