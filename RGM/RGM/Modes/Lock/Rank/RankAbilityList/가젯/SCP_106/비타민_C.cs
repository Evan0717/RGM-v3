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

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("비타민 C", "즉시 기력을 20% 회복합니다.", RankAbilityType.비타민_C, RankCategory.SCP_106, "🍺")]
    public class 비타민_C : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Owner.Role is Scp106Role scp106 && scp106.Vigor < scp106.VigorAbility.Vigor.MaxValue)
                return true;

            else
                return false;
        }

        protected override void OnGadgetUsed()
        {
            if (Owner.Role is Scp106Role scp106)
                scp106.Vigor += scp106.VigorAbility.Vigor.MaxValue * 0.2f;
        }
    }
}
