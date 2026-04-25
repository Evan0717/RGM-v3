using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using MEC;
using MultiBroadcast.Commands.Subcommands;
using PlayerRoles;
using RGM.Modes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("참호", "근처에 있는 문을 닫고, 5초간 잠급니다.", RankAbilityType.참호, RankCategory.SCP_106, "🛅", 70)]
    public class 참호 : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Door.List.Count(x => Vector3.Distance(x.Position, Owner.Position) < 5) > 0)
                return true;

            else
                return false;
        }

        protected override void OnGadgetUsed()
        {
            foreach (BreakableDoor door in Door.List.Where(x => Vector3.Distance(x.Position, Owner.Position) < 5).Select(x => x as BreakableDoor).ToList())
            {
                door.IsOpen = false;
                door.Lock(5, DoorLockType.Regular079);
            }
        }
    }
}
