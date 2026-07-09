using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using RGM.Modes;
using System.Linq;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("참호", "근처에 있는 문을 닫고, 5초간 잠급니다.", RankAbilityType.참호, RankCategory.SCP_106, "🛅", 70)]
    public class 참호 : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Door.List.Any(x => Vector3.Distance(x.Position, Owner.Position) < 5 && !x.Type.ToString().Contains("Scp079")))
                return true;

            else
                return false;
        }

        protected override void OnGadgetUsed()
        {
            foreach (Door door in Door.List.Where(x => Vector3.Distance(x.Position, Owner.Position) < 5 && !x.Type.ToString().Contains("Scp079")).ToList())
            {
                door.IsOpen = false;
                door.Lock(5, DoorLockType.Regular079);
            }
        }
    }
}
