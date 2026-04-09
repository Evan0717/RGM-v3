using Exiled.API.Extensions;
using Exiled.API.Features;
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
    [RankGadget("유대", "즉시 SCP-049 위치로 이동합니다.", RankAbilityType.유대, RankCategory.SCP_049_2, "💌")]
    public class 유대 : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            return Player.List.Count(x => x.Role.Type == RoleTypeId.Scp049) > 0;
        }

        protected override void OnGadgetUsed()
        {
            Owner.Position = Player.List.FirstOrDefault(x => x.Role.Type == RoleTypeId.Scp049).Position;
        }
    }
}
