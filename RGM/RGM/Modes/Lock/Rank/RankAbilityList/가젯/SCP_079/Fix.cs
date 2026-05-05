using Exiled.API.Extensions;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Roles;
using RGM.Modes;
using System.Collections.Generic;
using System.Linq;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("Fix", "보고 있는 방의 부서진 문 중 하나를 복구합니다.", RankAbilityType.Fix, RankCategory.SCP_079, "🔨")]
    public class Fix : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Owner.Role is Scp079Role scp079 && scp079.Camera.Room.Doors.Count(x => x is BreakableDoor door && door.IsDestroyed) > 0)
                return true;

            else
                return false;
        }

        protected override void OnGadgetUsed()
        {
            if (Owner.Role is Scp079Role scp079)
            {
                List<BreakableDoor> doors = scp079.Camera.Room.Doors.Where(x => x is BreakableDoor door && door.IsDestroyed).Select(x => x as BreakableDoor).ToList();
                if (doors.Count > 0)
                {
                    BreakableDoor door = doors.GetRandomValue();
                    door.Repair();
                }
            }
        }
    }
}
