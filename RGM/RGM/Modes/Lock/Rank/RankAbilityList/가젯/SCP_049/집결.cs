using Exiled.API.Features;
using PlayerRoles;
using RGM.Modes;
using System.Collections.Generic;
using System.Linq;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("집결", "SCP-049-2들을 전부 SCP-049의 위치로 이동시킵니다.", RankAbilityType.집결, RankCategory.SCP_049, "👪", 90)]
    public class 집결 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            List<Player> scp0492s = Player.List.Where(p => p.Role == RoleTypeId.Scp0492).ToList();

            foreach (var player in scp0492s)
                player.Position = Owner.Position;
        }
    }
}
