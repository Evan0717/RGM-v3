using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using RGM.Modes;
using UnityEngine;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.변칙성
{
    [RankAbility("스피드런", "시설 관리자 키카드를 얻으며, 즉시 고위험군으로 이동합니다.", RankAbilityType.스피드런, RankCategory.과학자, RankAbilityCategory.변칙성, "🔬")]
    public class 스피드런 : RankAbility
    {
        public override void OnEnabled()
        {
            Owner.AddItem(ItemType.KeycardFacilityManager);
            Owner.Position = Door.Get(DoorType.ElevatorServerRoom).Position + Vector3.up * 1.5f;
        }

        public override void OnDisabled()
        {
        }
    }
}
