using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp173;
using MEC;
using MultiBroadcast.Commands.Subcommands;
using PlayerRoles;
using RGM.Modes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("부드럽고 따뜻한 호박 고구마", "즉시 웅덩이를 만듭니다.", RankAbilityType.부드럽고_따뜻한_호박_고구마, RankCategory.SCP_173, "🍠")]
    public class 부드럽고_따뜻한_호박_고구마 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            if (Owner.Role is Scp173Role scp173)
                scp173.PlaceTantrum();
        }
    }
}
