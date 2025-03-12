using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RGM.API;
using Achievements.Handlers;
using Exiled.API.Enums;
using UnityEngine;
using CustomPlayerEffects;
using MultiBroadcast.API;
using RGM.API.Interfaces;
using RGM.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Extensions;

namespace RGM.Modes
{
    [Mode(ModeCategory.Public, ModeInfo.Set, ModeType.WhoamI2)]
    public class WhoamI2 : Mode
    {
        public override string Name => "누구게?";
        public override string Description => "1분마다 진영이 변경됩니다.";
        public override string Detail =>
"""
1분마다 진영이 변경된다는 설명으로도 충분하다.
""";
        public override string Color => "01DF74";

        public static WhoamI2 Instance;

        public List<RoleTypeId> ignoredRoles = new List<RoleTypeId>
        {
            RoleTypeId.Spectator,
            RoleTypeId.Overwatch,
            RoleTypeId.Filmmaker,
            RoleTypeId.None,
            RoleTypeId.Flamingo,
            RoleTypeId.AlphaFlamingo,
            RoleTypeId.ZombieFlamingo,
            RoleTypeId.Destroyed
        };

        public override void OnEnabled()
        {
            Timing.RunCoroutine(OnModeStarted());
        }

        public IEnumerator<float> OnModeStarted()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(60f);

                foreach (var player in Player.List.Where(x => !ignoredRoles.Contains(x.Role.Type)).ToList())
                    player.Role.Set(Tools.EnumToList<RoleTypeId>().Where(x => !ignoredRoles.Contains(x)).ToList().GetRandomValue(), RoleSpawnFlags.None);
            }
        }
    }
}
