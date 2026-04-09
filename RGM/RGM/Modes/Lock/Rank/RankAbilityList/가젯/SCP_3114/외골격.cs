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
    [RankGadget("외골격", "흄 쉴드를 모두 소모하여 50% 만큼 체력으로 치환합니다.", RankAbilityType.외골격, RankCategory.SCP_3114, "💀")]
    public class 외골격 : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Owner.MaxHumeShield <= Owner.HumeShield)
                return false;

            else
                return true;
        }
        protected override void OnGadgetUsed()
        {
            float hume = Owner.HumeShield;

            Owner.HumeShield = 0;

            Owner.Heal(hume / 2);
        }
    }
}
