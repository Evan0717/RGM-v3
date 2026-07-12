using Exiled.API.Extensions;
using Exiled.API.Features;
using RGM.Modes;
using System.Linq;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("변칙성 이동기", "랜덤한 아군에게 순간이동합니다.", RankAbilityType.변칙성_이동기, RankCategory.튜토리얼, "📞", 120)]
    public class 변칙성_이동기 : RankGadgetAbility
    {
        protected override bool CanUseGadget()
        {
            if (Player.List.Count(x => !HitboxIdentity.IsEnemy(x.ReferenceHub, Owner.ReferenceHub) && x != Owner) == 0)
                return false;

            else
                return true;
        }
        protected override void OnGadgetUsed()
        {
            Player player = Player.List.GetRandomValue(x => !HitboxIdentity.IsEnemy(x.ReferenceHub, Owner.ReferenceHub) && x != Owner);

            Owner.Position = player.Position;
        }
    }
}
