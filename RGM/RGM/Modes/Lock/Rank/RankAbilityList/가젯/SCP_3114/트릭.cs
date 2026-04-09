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
using RGM.API.Features;
using RGM.Modes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("트릭", "변장하지 않은 상태여도 20초간 인간으로 보입니다.", RankAbilityType.트릭, RankCategory.SCP_3114, "🃏")]
    public class 트릭 : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Owner.Role is Scp3114Role scp3114 && (
                scp3114.DisguiseStatus == PlayerRoles.PlayableScps.Scp3114.Scp3114Identity.DisguiseStatus.Active ||
                scp3114.DisguiseStatus == PlayerRoles.PlayableScps.Scp3114.Scp3114Identity.DisguiseStatus.Equipping
                ))
                return false;

            else
                return true;
        }
        protected override void OnGadgetUsed()
        {
            Owner.ChangeAppearance(Tools.EnumToList<RoleTypeId>().Where(x => x.IsHuman() && x != RoleTypeId.Tutorial).GetRandomValue());

            Timing.CallDelayed(20, () =>
            {
                Owner.ChangeAppearance(RoleTypeId.Scp3114);
            });
        }
    }
}
