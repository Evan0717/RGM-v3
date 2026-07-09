using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using RGM.API.Features;
using RGM.Modes;
using System.Linq;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("유독성 가스", "안개에 있는 적들에게 4초간 부식 효과를 적용하고 40의 데미지를 입힙니다.", RankAbilityType.유독성_가스, RankCategory.SCP_939, "🚫", 90)]
    public class 유독성_가스 : RankGadgetAbility
    {
        protected override void OnGadgetUsed()
        {
            foreach (var human in Player.List.Where(x => x.IsHuman && x.IsEffectActive<AmnesiaVision>()).ToList())
            {
                human.Hit(Owner, 40);
                human.AddEffect(EffectType.Corroding, 1, 4);
            }
        }
    }
}
