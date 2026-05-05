using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using RGM.API.Features;
using RGM.Modes;
using System.Linq;

namespace RGM.RGM.Modes.Lock.Rank.RankAbilityList.가젯
{
    [RankGadget("Brake", "보고 있는 방의 인간들에게 2초간 부식 효과를 적용합니다.", RankAbilityType.Brake, RankCategory.SCP_079, "🔮", 110)]
    public class Brake : RankGadgetAbility
    {

        protected override void OnGadgetUsed()
        {
            if (Owner.Role is Scp079Role scp079)
            {
                foreach (var player in scp079.Camera.Room.Players.Where(x => x.IsHuman).ToList())
                    player.AddEffect(EffectType.Corroding, 1, 2);
            }
        }
    }
}
